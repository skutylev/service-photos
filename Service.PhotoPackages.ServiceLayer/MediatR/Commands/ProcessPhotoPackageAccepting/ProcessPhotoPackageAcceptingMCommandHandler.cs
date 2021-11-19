using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Service.PhotoPackages.Dal;
using Service.PhotoPackages.Dal.Models;
using Service.PhotoPackages.ServiceLayer.Constants;
using Service.PhotoPackages.ServiceLayer.MediatR.Commands.SendImagesToScanServer;
using Service.PhotoPackages.ServiceLayer.MediatR.Commands.SendPhotoDocumentsOnEmail;
using Service.PhotoPackages.ServiceLayer.MediatR.Commands.SendPushes;

namespace Service.PhotoPackages.ServiceLayer.MediatR.Commands.ProcessPhotoPackageAccepting
{
    public class ProcessPhotoPackageAcceptingMCommandHandler : AsyncRequestHandler<ProcessPhotoPackageAcceptingMCommand>
    {
        private readonly PhotoPackagesDbContext _dbContext;
        private readonly IMediator _mediator;


        public ProcessPhotoPackageAcceptingMCommandHandler(PhotoPackagesDbContext dbContext, IMediator mediator)
        {
            _dbContext = dbContext;
            _mediator = mediator;
        }

        protected override async Task Handle(ProcessPhotoPackageAcceptingMCommand request,
            CancellationToken cancellationToken)
        {
            if (!request.TypeCode.Equals(PhotoPackageTypes.Contract) || !request.ProcessingCompleted) return;

            var photoPackage = await GetPhotoPackage(request.ContractId, request.PhotoPackageId, cancellationToken);

            var needSendToTw = new List<string>
            {
                PhotoPackageProcessTypes.CreditSigned, 
                PhotoPackageProcessTypes.DebitSigned,
                PhotoPackageProcessTypes.DebitOffline
            }.Contains(photoPackage.ProcessCode);
            
            var tasks = new List<Task>
            {
                _mediator.Send(new SendPhotoDocumentsOnEmailMCommand
                {
                    ContractId = photoPackage.ContractId,
                    SapNumber = request.CourierSapNumber
                }, cancellationToken)
            };

            if (PhotoPackageProcessTypes.VerifiableProcesses.Contains(photoPackage.ProcessCode))
                tasks.Add(_mediator.Send(new SendPushAcceptedMCommand
                {
                    ContractId = photoPackage.ContractId,
                    ReceiverSapNumber = request.CourierSapNumber
                }, cancellationToken));
            
            if (!photoPackage.ProcessCode.Equals(PhotoPackageProcessTypes.DebitOnline))
                tasks.Add(_mediator.Send(new SendPhotosToScanServerMCommand
                {
                    Id = photoPackage.Id,
                    ContractId = photoPackage.ContractId,
                    BucketId = photoPackage.BucketId,
                    TypeCode = photoPackage.TypeCode,
                    NeedSendToTw = needSendToTw,
                    ProcessTypeCode = photoPackage.ProcessCode,
                    PhotosInfo = photoPackage.Photos.Where(p => !string.IsNullOrEmpty(p.ContentId)).Select(p => (p.Id, p.TypeCode, p.ContentId))
                }, cancellationToken));

            await Task.WhenAll(tasks);
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