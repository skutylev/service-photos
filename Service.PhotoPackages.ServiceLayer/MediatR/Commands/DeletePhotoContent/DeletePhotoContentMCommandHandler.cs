using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Service.PhotoPackages.Dal;

namespace Service.PhotoPackages.ServiceLayer.MediatR.Commands.DeletePhotoContent
{
    public class DeletePhotoContentMCommandHandler : AsyncRequestHandler<DeletePhotoContentMCommand>
    {
        private readonly PhotoPackagesDbContext _dbContext;

        public DeletePhotoContentMCommandHandler(PhotoPackagesDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        protected override async Task Handle(DeletePhotoContentMCommand request, CancellationToken cancellationToken)
        {
            var photo = await _dbContext.Photos
                            .Include(p => p.PhotoPackage)
                            .FirstOrDefaultAsync(p =>
                                    p.Id == request.PhotoId &&
                                    p.PhotoPackage.ContractId == request.ContactId &&
                                    p.PhotoPackage.Id == request.PhotoPackageId,
                                cancellationToken) ??
                        throw new ArgumentOutOfRangeException(nameof(request.PhotoPackageId), "Фото не найдено");

            if (photo.Required)
                throw new ArgumentOutOfRangeException(nameof(request.PhotoPackageId),
                    "Удаление обязательного фото запрещено");
            
            if (photo.ContentId == null)
                throw new ArgumentOutOfRangeException(nameof(request.PhotoPackageId),
                    "Невозможно удалить фото с пустым контентом");
            
            photo.StatusCode = PhotoStatuses.New;
            photo.ContentId = null;
            photo.ThumbnailContentId = null;
            photo.CanBeRetaken = true;
            photo.ErrorCode = null;
            photo.ErrorMessage = null;
            photo.ScanServerDocumentId = null;
            photo.ScanServerPageId = null;
            photo.ScanServerPageNumber = null;
            photo.TranzWareScanId = null;

            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}