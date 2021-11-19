using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MassTransit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Service.PhotoPackages.Dal;
using Service.PhotoPackages.Dal.Models;
using Service.PhotoPackages.Events;

namespace Service.PhotoPackages.ServiceLayer.MediatR.Commands.ClearPhotoPackage
{
    public class ClearPhotoPackageMCommandHandler : AsyncRequestHandler<ClearPhotoPackageMCommand>
    {
        private readonly PhotoPackagesDbContext _dbContext;
        private readonly IBus _bus;

        public ClearPhotoPackageMCommandHandler(PhotoPackagesDbContext dbContext, IBus bus)
        {
            _dbContext = dbContext;
            _bus = bus;
        }

        protected override async Task Handle(ClearPhotoPackageMCommand request, CancellationToken cancellationToken)
        {
            var photoPackages = await GetPhotoPackages(request.ContractId, cancellationToken);
            
            foreach (var photoPackage in photoPackages)
            {
                photoPackage.StatusCode = PhotoPackageStatuses.New;
                photoPackage.ScanCheckerRequestId = null;
                photoPackage.Latitude = null;
                photoPackage.Longitude = null;
                photoPackage.ErrorCode = null;
                photoPackage.ErrorMessage = null;
                photoPackage.ProcessCode = null;
                photoPackage.AuthorSapNumber = request.SapNumber;

                foreach (var photo in photoPackage.Photos)
                {
                    photo.ContentId = null;
                    photo.ThumbnailContentId = null;
                    photo.ErrorCode = null;
                    photo.ErrorMessage = null;
                    photo.StatusCode = PhotoStatuses.New;
                    photo.CanBeRetaken = true;
                    photo.ScanServerDocumentId = null;
                    photo.ScanServerPageId = null;
                    photo.ScanServerPageNumber = null;
                    photo.TranzWareScanId = null;
                }
            }

            await _dbContext.SaveChangesAsync(cancellationToken);

            await _bus.Publish(new PhotoPackageClearedEvent
            {
                TypeCode = PhotoPackageTypes.Contract,
                AuthorSapNumber = request.SapNumber,
                ContractId = request.ContractId
            }, cancellationToken);
        }
        
        private async Task<IEnumerable<PhotoPackage>> GetPhotoPackages(string contractId,
            CancellationToken cancellationToken)
        {
            return await _dbContext.PhotoPackages
                       .Include(p => p.Photos)
                       .Where(pp => pp.ContractId == contractId)
                       .ToListAsync(cancellationToken);
        }
    }
}