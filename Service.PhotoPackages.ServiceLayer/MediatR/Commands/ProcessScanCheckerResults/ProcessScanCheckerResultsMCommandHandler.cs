using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Service.Integrations.Client.Contracts.ScanCheckerModels;
using Service.PhotoPackages.Dal;
using Service.PhotoPackages.Dal.Models;
using Service.PhotoPackages.ServiceLayer.Constants;
using Service.PhotoPackages.ServiceLayer.Integrations.Contracts;
using Service.PhotoPackages.ServiceLayer.MediatR.Commands.ProcessScanCheckerResults.Models;
using Service.PhotoPackages.ServiceLayer.MediatR.Commands.UpdatePhotoPackageStatus;

namespace Service.PhotoPackages.ServiceLayer.MediatR.Commands.ProcessScanCheckerResults
{
    public class ProcessScanCheckerResultsMCommandHandler : AsyncRequestHandler<ProcessScanCheckerResultsMCommand>
    {
        private readonly PhotoPackagesDbContext _dbContext;
        private readonly IScanCheckerService _scanCheckerService;
        private readonly ISettingsService _settingsService;
        private readonly IMediator _mediator;
        private const string NoClientSignError = "NO_CLIENT_SIGN";

        public ProcessScanCheckerResultsMCommandHandler(PhotoPackagesDbContext dbContext,
            IScanCheckerService scanCheckerService, IMediator mediator, ISettingsService settingsService)
        {
            _dbContext = dbContext;
            _scanCheckerService = scanCheckerService;
            _mediator = mediator;
            _settingsService = settingsService;
        }

        protected override async Task Handle(ProcessScanCheckerResultsMCommand request,
            CancellationToken cancellationToken)
        {
            var processingPhotoPackages = await GetProcessingPhotoPackages(cancellationToken);
            if (!processingPhotoPackages.Any()) return;

            var verificationResults =
                await _scanCheckerService.CheckPhotoStatuses(
                    processingPhotoPackages.Select(p => p.ScanCheckerRequestId), cancellationToken);

            if (verificationResults == null || !verificationResults.ScanCheckerResults.Any()) return;

            foreach (var verificationResult in verificationResults.ScanCheckerResults)
            {
                if (!new List<string> {"VERIFICATION_PASSED", "VERIFICATION_FAILED"}
                    .Contains(verificationResult.CheckingResult))
                    continue;
                
                var photoPackage = processingPhotoPackages.FirstOrDefault(p =>
                    p.ScanCheckerRequestId == verificationResult.ScanCheckerRequestId);
                if( photoPackage == null ) continue;
                
                await ProcessVerification(photoPackage, verificationResult, cancellationToken);
            }
        }

        private Task<List<PhotoPackage>> GetProcessingPhotoPackages(CancellationToken cancellationToken) => _dbContext
            .PhotoPackages
            .Include(p => p.Photos)
            .Where(p => p.StatusCode == PhotoPackageStatuses.Pending
                        && p.ScanCheckerRequestId != null)
            .AsNoTracking()
            .ToListAsync(cancellationToken);


        private async Task ProcessVerification(PhotoPackage photoPackage, CheckPhotoStatusResult result,
            CancellationToken cancellationToken)
        {
            var scanCheckerResult = MapResultsOnPhotoPackage(photoPackage, result);

            await _mediator.Send(new UpdateScanServerDataMCommand.UpdateScanServerDataMCommand
            {
                PhotoPackageId = scanCheckerResult.PhotoPackageId,
                PhotoInfos = scanCheckerResult.PhotosResults.Select(pr =>
                    new UpdateScanServerDataMCommand.UpdateScanServerDataMCommand.PhotoScanServerInfo
                    {
                        PhotoId = pr.PhotoId,
                        ScanServerDocumentId = pr.ScanServerDocumentId,
                        ScanServerPageId = pr.ScanServerPageId,
                        ScanServerPageNumber = pr.ScanServerPageNumber
                    })
            }, cancellationToken);

            await _mediator.Send(new UpdatePhotoPackageStatusMCommand
            {
                Id = scanCheckerResult.PhotoPackageId,
                ContractId = photoPackage.ContractId,
                CallerType = CallerTypes.System,
                StatusCode = scanCheckerResult.PhotoPackageStatusCode,
                ErrorCode = scanCheckerResult.PhotoPackageErrorCode,
                ErrorMessage = scanCheckerResult.PhotoPackageErrorMessage,
                PhotoErrors = scanCheckerResult.PhotosResults.Where(p => !string.IsNullOrEmpty(p.PhotoId))
                    .Select(p => (p.PhotoId, p.PhotoErrorCode, p.PhotoErrorMessage, p.PhotoStatusCode)).ToList()
            }, cancellationToken);
        }


        private ScanCheckerPhotoPackageResult MapResultsOnPhotoPackage(PhotoPackage photoPackage,
            CheckPhotoStatusResult scanCheckerResult)
        {
            if (!string.IsNullOrEmpty(scanCheckerResult.Comment) &&
                string.IsNullOrEmpty(scanCheckerResult.FailedMessage))
                scanCheckerResult.FailedMessage = "REJECT_OD";

            return new ScanCheckerPhotoPackageResult
            {
                PhotoPackageId = photoPackage.Id,
                ScanCheckerRequestId = scanCheckerResult.ScanCheckerRequestId,
                PhotoPackageStatusCode = GetPhotoPackageStatusFromScanCheckerResult(scanCheckerResult.CheckingResult,
                    photoPackage.TypeCode, scanCheckerResult.PhotoResults.Where(p => !string.IsNullOrEmpty(p.ErrorCode)).Select(p => p.ErrorCode)),
                PhotoPackageErrorCode = scanCheckerResult.FailedMessage,
                PhotoPackageErrorMessage = !string.IsNullOrEmpty(scanCheckerResult.FailedMessage)
                    ? GetErrorMessage(scanCheckerResult.FailedMessage, scanCheckerResult.Comment) : null,
                PhotosResults = scanCheckerResult.PhotoResults.Select(p => new
                {
                    PhotoInfos = p.ScanServerPageIds.OrderBy(r => r)
                        .Zip(GetPhotoTypeByScanServerDocIdent(p.ScanCheckerPhotoType, !string.IsNullOrEmpty(p.OtherSign))),
                    DocumentId = p.ScanServerDocumentId,
                    ErrorCode = p.ErrorCode,
                    p.ScanCheckerPhotoType
                }).SelectMany(p => p.PhotoInfos.Select(r => new ScanCheckerPhotoResult
                {
                    PhotoId = photoPackage.Photos.FirstOrDefault(pp => pp.TypeCode.Equals(r.Second))?.Id,
                    PhotoTypeCode = r.Second,
                    PhotoStatusCode = GetPhotoStatusFromScanCheckerResult(scanCheckerResult.CheckingResult, p.ErrorCode),
                    PhotoErrorCode =  p.ErrorCode,
                    PhotoErrorMessage = !string.IsNullOrEmpty(p.ErrorCode) ? GetErrorMessage(p.ErrorCode) : null,
                    ScanCheckerPhotoType = p.ScanCheckerPhotoType,
                    ScanServerPageNumber = "1",
                    ScanServerPageId = r.First.ToString(),
                    ScanServerDocumentId = p.DocumentId
                }))
            };
        }

        private string GetPhotoPackageStatusFromScanCheckerResult(string checkingResult, string photoPackageTypeCode,
            IEnumerable<string> photoErrors) =>
            checkingResult switch
            {
                "VERIFICATION_FAILED" when !photoPackageTypeCode.Equals(PhotoPackageTypes.Contract) &&
                                           photoErrors.Contains(NoClientSignError,
                                               StringComparer.InvariantCultureIgnoreCase) => PhotoPackageStatuses
                    .Accepted,
                "VERIFICATION_FAILED" => PhotoPackageStatuses.Rejected,
                "VERIFICATION_PASSED" => PhotoPackageStatuses.Accepted,
                _ => throw new NotImplementedException()
            };

        private string GetPhotoStatusFromScanCheckerResult(string checkingResult, string photoErrorCode) =>
            checkingResult.Equals("VERIFICATION_FAILED") && !string.IsNullOrEmpty(photoErrorCode)
                ? PhotoStatuses.Rejected
                : PhotoStatuses.Accepted;

        private IEnumerable<string> GetPhotoTypeByScanServerDocIdent(string scanServerDocIdent, bool otherSign)
        {
            var photoTypes = _settingsService.GetPhotoTypes().Where(p =>
                p.Properties != null &&
                p.Properties.Any(pp =>
                    pp.Name.Equals(PhotoTypeProperties.ScanCheckerDocumentType) &&
                    pp.Value.Equals(scanServerDocIdent)));

            photoTypes = otherSign
                ? photoTypes.Where(p => p.Properties.Any(pp =>
                    pp.Name.Equals(PhotoTypeProperties.OtherSign)))
                : photoTypes.Where(p => !p.Properties.Any(pp =>
                    pp.Name.Equals(PhotoTypeProperties.OtherSign)));

            return photoTypes.OrderBy(p => p.Flag).Select(p => p.Code);
        }

        private string GetErrorMessage(string errorCode, string comment = null)
        {
            if (errorCode == null) return null;
            
            var errorMessage = _settingsService.GetPhotoErrorTypes()
                .FirstOrDefault(p => p.Code.Equals(errorCode, StringComparison.InvariantCultureIgnoreCase))?.Name;

            return string.IsNullOrEmpty(comment) ? errorMessage : $"{errorMessage}: {comment}";
        }
    }
}