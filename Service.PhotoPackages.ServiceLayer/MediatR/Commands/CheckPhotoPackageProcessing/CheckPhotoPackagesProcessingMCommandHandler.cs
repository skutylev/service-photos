using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Service.PhotoPackages.Dal;
using Service.PhotoPackages.Dal.Models;
using Service.PhotoPackages.ServiceLayer.Infrastructure;
using Service.PhotoPackages.ServiceLayer.MediatR.Commands.FinishPhotoPackageProcessing;

namespace Service.PhotoPackages.ServiceLayer.MediatR.Commands.CheckPhotoPackageProcessing
{
    public class CheckPhotoPackagesProcessingMCommandHandler : AsyncRequestHandler<CheckPhotoPackagesProcessingMCommand>
    {
        private readonly PhotoPackagesDbContext _dbContext;
        private readonly RedisStorage _redisStorage;

        private const string ProcessPrefix = "processing";
        private const string NoClientSignError = "NO_CLIENT_SIGN";
        private readonly IMediator _mediator;

        public CheckPhotoPackagesProcessingMCommandHandler(PhotoPackagesDbContext dbContext, RedisStorage redisStorage,
            IMediator mediator)
        {
            _dbContext = dbContext;
            _redisStorage = redisStorage;
            _mediator = mediator;
        }

        protected override async Task Handle(CheckPhotoPackagesProcessingMCommand request,
            CancellationToken cancellationToken)
        {
            var contractIds = (await _redisStorage.GetKeysByProcess<string>(ProcessPrefix)).Keys;
            var photoPackagesByContracts = await GetPhotoPackageGroupedByContactId(contractIds, cancellationToken);

            foreach (var photoPackagesByContract in photoPackagesByContracts)
            {
                var contract = photoPackagesByContract.Value.FirstOrDefault(photoPackage =>
                    photoPackage.TypeCode == PhotoPackageTypes.Contract);

                var lk = photoPackagesByContract.Value.FirstOrDefault(photoPackage =>
                    photoPackage.TypeCode == PhotoPackageTypes.ApplicationLk);

                var vzr = photoPackagesByContract.Value.FirstOrDefault(photoPackage =>
                    photoPackage.TypeCode == PhotoPackageTypes.ApplicationVzr);

                if (
                    contract is {StatusCode: PhotoPackageStatuses.Accepted} &&
                    (
                        lk == null ||
                        lk.StatusCode == PhotoPackageStatuses.Accepted ||
                        lk.StatusCode == PhotoPackageStatuses.Rejected &&
                        !string.IsNullOrWhiteSpace(lk.ErrorCode) && lk.ErrorCode.Equals(NoClientSignError) ||
                        lk.Photos.Any(photo => !string.IsNullOrWhiteSpace(photo.ErrorCode) && photo.ErrorCode.Equals(NoClientSignError))
                    )
                    &&
                    (
                        vzr == null ||
                        vzr.StatusCode == PhotoPackageStatuses.Accepted ||
                        vzr.StatusCode == PhotoPackageStatuses.Rejected &&
                        !string.IsNullOrWhiteSpace(vzr.ErrorCode) && vzr.ErrorCode.Equals(NoClientSignError) ||
                        vzr.Photos.Any(photo => !string.IsNullOrWhiteSpace(photo.ErrorCode) && photo.ErrorCode.Equals(NoClientSignError))
                    )
                )
                    await _mediator.Send(new FinishPhotoPackageProcessingMCommand()
                    {
                        Id = contract.Id,
                        ContractId = contract.ContractId,
                        CourierSapNumber = contract.AuthorSapNumber ?? 0,
                        AuthorSapNumber = contract.VerifierSapNumber ?? 0,
                        ProcessTypeCode = contract.ProcessCode,
                        TypeCode = contract.TypeCode
                    }, cancellationToken);
            }
        }

        private async Task<IDictionary<string, List<PhotoPackage>>> GetPhotoPackageGroupedByContactId(
            IEnumerable<string> contractIds, CancellationToken cancellationToken)
        {
            return (await _dbContext.PhotoPackages
                .Where(p => contractIds.Contains(p.ContractId))
                .Include(p => p.Photos)
                .AsNoTracking()
                .ToListAsync(cancellationToken))
                .GroupBy(p => p.ContractId)
                .ToDictionary(p => p.Key, p => p.ToList());
        }
    }
}