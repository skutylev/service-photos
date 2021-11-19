using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MassTransit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Service.PhotoPackages.Dal;
using Service.PhotoPackages.Dal.Models;
using Service.PhotoPackages.Events;
using Service.PhotoPackages.ServiceLayer.Constants;
using Service.PhotoPackages.ServiceLayer.MediatR.Commands.ProcessPhotoPackageSaving.Strategy;
using Service.PhotoPackages.ServiceLayer.MediatR.Commands.UpdatePhotoPackageStatus;

namespace Service.PhotoPackages.ServiceLayer.MediatR.Commands.ProcessPhotoPackageSaving
{
    public class ProcessPhotoPackageSavedEventMCommandHandler : AsyncRequestHandler<ProcessPhotoPackageSavedEventMCommand>
    {
        private readonly PhotoPackagesDbContext _dbContext;
        private readonly IPackagePhotoProcessStrategyManager _strategyManager;
        private readonly IMediator _mediator;
        private readonly IBus _bus;

        public ProcessPhotoPackageSavedEventMCommandHandler(PhotoPackagesDbContext dbContext,
            IPackagePhotoProcessStrategyManager strategyManager, IBus bus, IMediator mediator)
        {
            _dbContext = dbContext;
            _strategyManager = strategyManager;
            _bus = bus;
            _mediator = mediator;
        }

        protected override async Task Handle(ProcessPhotoPackageSavedEventMCommand request,
            CancellationToken cancellationToken)
        {
            var photoPackage = await GetPhotoPackage(request.ContractId, request.PhotoPackageId, cancellationToken);
            if (photoPackage.StatusCode != PhotoPackageStatuses.Saved) return;
            
            photoPackage.StatusCode = PhotoPackageStatuses.Pending;

            var processResult = await _strategyManager.Get(photoPackage.ProcessCode).Apply(photoPackage, cancellationToken);
            photoPackage.ScanCheckerRequestId = processResult.ScanCheckerRequestId;

            await _dbContext.SaveChangesAsync(cancellationToken);
            
            await _bus.Publish(new PhotoPackagePendedEvent
            {
                ContractId = photoPackage.ContractId,
                Id = photoPackage.Id,
                AuthorSapNumber = photoPackage.VerifierSapNumber ?? 0,
                CourierSapNumber = photoPackage.AuthorSapNumber ?? 0,
                ProcessTypeCode = photoPackage.ProcessCode,
                TypeCode = photoPackage.TypeCode,
            }, cancellationToken);
            
            if (processResult.PhotoPackageStatusCode.Equals(PhotoPackageStatuses.Pending)) 
                return;
            
            await _mediator.Send(new UpdatePhotoPackageStatusMCommand
            {
                Id = photoPackage.Id,
                ContractId = photoPackage.ContractId,
                CallerType = CallerTypes.System,
                StatusCode = processResult.PhotoPackageStatusCode,
                ErrorCode = photoPackage.ErrorCode,
                ErrorMessage = photoPackage.ErrorMessage,
                PhotoErrors = photoPackage.Photos?.Select(e => (e.Id, e.ErrorCode, e.ErrorMessage, processResult.PhotoPackageStatusCode)).ToList()
            }, cancellationToken);

        }

        private async Task<PhotoPackage> GetPhotoPackage(string contractId, string packagePhotoId,
            CancellationToken cancellationToken)
        {
            return await _dbContext.PhotoPackages
                       .Include(p => p.History)
                       .Include(p => p.Photos)
                       .ThenInclude(p => p.History)
                       .FirstOrDefaultAsync(pp =>
                           pp.ContractId == contractId && pp.Id == packagePhotoId, cancellationToken) ??
                   throw new ArgumentOutOfRangeException(nameof(packagePhotoId),
                       "Пакет фото с указанным PackagePhotoId не найден");
        }
    }
}