using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Service.PhotoPackages.Dal;
using Service.PhotoPackages.Dal.Models;
using Service.PhotoPackages.ServiceLayer.MediatR.Commands.SendPushes;

namespace Service.PhotoPackages.ServiceLayer.MediatR.Commands.ProcessPhotoPackageRejecting
{
    public class ProcessPhotoPackageRejectingMCommandHandler : AsyncRequestHandler<ProcessPhotoPackageRejectingMCommand>
    {
        private readonly PhotoPackagesDbContext _dbContext;
        private readonly IMediator _mediator;

        public ProcessPhotoPackageRejectingMCommandHandler(PhotoPackagesDbContext dbContext, IMediator mediator)
        {
            _dbContext = dbContext;
            _mediator = mediator;
        }

        protected override async Task Handle(ProcessPhotoPackageRejectingMCommand request, CancellationToken cancellationToken)
        {
            var photoPackage = await GetPhotoPackage(request.ContractId, request.PhotoPackageId, cancellationToken);
            await Task.WhenAll(new List<Task>
            {
                _mediator.Send(new SendPushRejectedMCommand()
                {
                    ContractId = photoPackage.ContractId,
                    ReceiverSapNumber = request.CourierSapNumber
                }, cancellationToken)
            });
        }
        
        private async Task<PhotoPackage> GetPhotoPackage(string contractId, string photoPackageId,
            CancellationToken cancellationToken)
        {
            return await _dbContext.PhotoPackages
                .Include(pp => pp.Photos)
                .FirstOrDefaultAsync(p =>
                        p.Id == photoPackageId &&
                        p.ContractId == contractId,
                    cancellationToken) ?? throw new ArgumentOutOfRangeException(nameof(photoPackageId),
                $"Пакет фотографий {photoPackageId} не найден");
        }
    }
}