using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Service.Contracts.Client.Contracts.Contracts;
using Service.Integrations.Client.Contracts.ScanCheckerModels;
using Service.PhotoPackages.Dal;
using Service.PhotoPackages.Dal.Models;
using Service.PhotoPackages.ServiceLayer.Constants;
using Service.PhotoPackages.ServiceLayer.Integrations.Contracts;
using Service.PhotoPackages.ServiceLayer.Integrations.DMNs.ScanCheckerProcessTypes;

namespace Service.PhotoPackages.ServiceLayer.MediatR.Commands.ProcessPhotoPackageSaving.Strategy
{
    public class DebitOnlinePackagePhotoProcessStrategy : IPackagePhotoProcessStrategy
    {
        private readonly IContractService _contractService;
        private readonly IUsersService _usersService;
        private readonly IScanCheckerService _scanCheckerService;
        private readonly IPhotoDmnService _photoDmnService;
        private readonly IBlobStorageService _blobStorageService;
        private readonly ISettingsService _settingsService;

        public DebitOnlinePackagePhotoProcessStrategy(IScanCheckerService scanCheckerService, IUsersService usersService, IContractService contractService, IPhotoDmnService photoDmnService, IBlobStorageService blobStorageService, ISettingsService settingsService)
        {
            _scanCheckerService = scanCheckerService;
            _usersService = usersService;
            _contractService = contractService;
            _photoDmnService = photoDmnService;
            _blobStorageService = blobStorageService;
            _settingsService = settingsService;
        }

        public async Task<ProcessResult> Apply(PhotoPackage photoPackage, CancellationToken cancellationToken)
        {
            var contractInfo = await _contractService.GetContractInfo(photoPackage.ContractId, cancellationToken);
            var user = (await _usersService.GetUsers(new[] { photoPackage.AuthorSapNumber ?? 0 }, cancellationToken)).FirstOrDefault();
            
            var scanCheckerProcessType =
                _photoDmnService.GetScanCheckerProcessTypes(new ScanCheckerProcessTypesDmnRequest
                {
                    ProductName = contractInfo.ProductName,
                    PhotoPackageTypeCode = photoPackage.TypeCode
                });

            var previousPhotoPackageHistoryItemWithError = photoPackage.History
                .Where(h => !string.IsNullOrEmpty(h.ErrorCode))
                .OrderByDescending(h => h.Id)
                .FirstOrDefault();

            var photosForSending = photoPackage.Photos.Select(photo => new
                {
                    ScanCheckerType = GetScanCheckerDocumentType(photo.TypeCode, contractInfo.ProductName),
                    OtherSign = GetOtherSign(photo.TypeCode),
                    photo.ContentId,
                    PreviousPhotoHistoryItemWithError = photo.History
                        .Where(h => !string.IsNullOrEmpty(h.ErrorCode))
                        .OrderByDescending(h => h.Id)
                        .FirstOrDefault(),
                }).Where(t => !string.IsNullOrEmpty(t.ScanCheckerType) && !string.IsNullOrEmpty(t.ContentId))
                .GroupBy(t => (t.ScanCheckerType, t.OtherSign))
                .Select(t => new
                {
                    ScanCheckerType = t.Key.ScanCheckerType,
                    OtherSign = t.Key.OtherSign,
                    ContentIds = t.Select(v => v.ContentId),
                    t.FirstOrDefault()?.PreviousPhotoHistoryItemWithError?.ErrorCode,
                    t.FirstOrDefault()?.PreviousPhotoHistoryItemWithError?.ErrorMessage,
                });

            var documents = new List<SendPhotosDocumentRequest>();

            foreach (var photo in photosForSending)
            {
                var document = new SendPhotosDocumentRequest
                {
                    ScanCheckerDocumentType = photo.ScanCheckerType,
                    PreviousErrorCode = photo.ErrorCode,
                    PreviousErrorMessage = photo.ErrorMessage,
                    OtherSign = photo.OtherSign,
                    PhotoContent = new List<byte[]>()
                };
                foreach (var contentId in photo.ContentIds)
                {
                    var blob = await _blobStorageService.GetPhoto(photoPackage.BucketId, contentId, cancellationToken);
                    document.PhotoContent.Add(blob);
                }
                documents.Add(document);
            }
            
            var sendPhotosRequest = new SendPhotosRequest
            {
                ContractNumber = GetEvidSrv(photoPackage.TypeCode, contractInfo),
                ClientFirstName = contractInfo.Client?.FirstName,
                ClientMiddleName = contractInfo.Client?.PatronymicName,
                ClientLastName = contractInfo.Client?.LastName,
                Ident = contractInfo.Client?.PassportNumber,
                CourierFirstName = user?.FirstName,
                CourierMiddleName = user?.PatronymicName,
                CourierLastName = user?.LastName,
                PartnerName = "MBR",
                PreviousError = previousPhotoPackageHistoryItemWithError?.ErrorCode,
                PreviousComment = BuildPreviousComment(previousPhotoPackageHistoryItemWithError?.ErrorMessage),
                Hint = BuildHint(photoPackage.TypeCode, contractInfo.ContractNumber, contractInfo.AdditionalServices, photoPackage.IsUnderage()),
                ScanCheckerProcessType = scanCheckerProcessType.ScanCheckerProcessTypeCode,
                Photos = documents
            };
            
            var scanCheckerRequestId = await _scanCheckerService.SendPhotos(sendPhotosRequest, cancellationToken);

            return new ProcessResult
            {
                PhotoPackageStatusCode = PhotoPackageStatuses.Pending,
                ScanCheckerRequestId = scanCheckerRequestId
            };
        }

        private string BuildHint(string photoPackageTypeCode, string contractNumber, Dictionary<string, string> additionalServiceCodes, bool isUnderage) =>
            photoPackageTypeCode switch
            {
                PhotoPackageTypes.Contract when isUnderage => "Клиент несовершеннолетний. Согласие родителя и доп. документы содержатся в группе фотографий \"Договор ДБО от TW\".",
                PhotoPackageTypes.Contract when
                    additionalServiceCodes != null && additionalServiceCodes.Keys.Contains("VZR") &&
                    additionalServiceCodes.Keys.Contains("LK") =>
                    $"К Договору {contractNumber} оформлена Услуга ВЗР. Не забудьте проверить Заявление {additionalServiceCodes.GetValueOrDefault("VZR")}. " +
                    $"К Договору {contractNumber} оформлена Услуга LK. Не забудьте проверить Заявление {additionalServiceCodes.GetValueOrDefault("LK")}.",
                PhotoPackageTypes.Contract when
                    additionalServiceCodes != null && additionalServiceCodes.Keys.Contains("LK") =>
                    $"К Договору {contractNumber} оформлена Услуга LK. Не забудьте проверить Заявление {additionalServiceCodes.GetValueOrDefault("LK")}.",
                PhotoPackageTypes.Contract when
                    additionalServiceCodes != null && additionalServiceCodes.Keys.Contains("VZR") =>
                    $"К Договору {contractNumber} оформлена Услуга ВЗР. Не забудьте проверить Заявление {additionalServiceCodes.GetValueOrDefault("VZR")}.",
                PhotoPackageTypes.ApplicationLk =>
                    $"Заявление на подключение услуги LK оформлено к Договору {contractNumber}.",
                PhotoPackageTypes.ApplicationVzr =>
                    $"Заявление на подключение услуги ВЗР оформлено к Договору {contractNumber}.",
                _ => null
            };

        private string BuildPreviousComment(string errorMessage)
        {
            if (string.IsNullOrWhiteSpace(errorMessage)) return null;

            return errorMessage.Length <= 255 ? errorMessage : errorMessage[..254];
        }

        private string GetScanCheckerDocumentType(string photoTypeCode, string productName)
        {
            var properties = _settingsService
                .GetPhotoTypes()
                .FirstOrDefault(t => t.Code == photoTypeCode)?
                .Properties
                .Where(p => p.Name.Contains(PhotoTypeProperties.ScanCheckerDocumentType, StringComparison.InvariantCultureIgnoreCase))
                .ToList();

            if (properties == null || !properties.Any()) return null;

            var forProduct = properties.FirstOrDefault(p =>
                p.Name.Contains($"{PhotoTypeProperties.ScanCheckerDocumentType}:{productName}",
                    StringComparison.InvariantCultureIgnoreCase));
            if (forProduct != null) return forProduct.Value;

            return properties.FirstOrDefault(p =>
                p.Name.Contains($"{PhotoTypeProperties.ScanCheckerDocumentType}",
                    StringComparison.InvariantCultureIgnoreCase))
                ?.Value;
        }

        private string GetEvidSrv(string photoPackageTypeCode, ContractItemDto contractInfo)
        {
            return photoPackageTypeCode switch
            {
                PhotoPackageTypes.Contract => contractInfo.ContractNumber,
                PhotoPackageTypes.ApplicationLk => contractInfo.AdditionalServices.GetValueOrDefault("LK"),
                PhotoPackageTypes.ApplicationVzr => contractInfo.AdditionalServices.GetValueOrDefault("VZR"),
                _ => throw new NotImplementedException($"PhotoPackageType {photoPackageTypeCode} not implemented")
            };
        }
        
        private string GetOtherSign(string photoTypeCode) =>
            _settingsService
                .GetPhotoTypes()
                .FirstOrDefault(t => t.Code == photoTypeCode)?
                .Properties
                .FirstOrDefault(p =>
                    p.Name.Contains(PhotoTypeProperties.OtherSign, StringComparison.InvariantCultureIgnoreCase))?.Value;
    }
}