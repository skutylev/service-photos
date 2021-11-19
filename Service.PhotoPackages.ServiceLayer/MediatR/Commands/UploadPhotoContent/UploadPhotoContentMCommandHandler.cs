using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Service.PhotoPackages.Client.Contracts;
using Service.PhotoPackages.Dal;
using Service.PhotoPackages.ServiceLayer.Constants;
using Service.PhotoPackages.ServiceLayer.Integrations.Contracts;
using Service.PhotoPackages.ServiceLayer.Utils;

namespace Service.PhotoPackages.ServiceLayer.MediatR.Commands.UploadPhotoContent
{
    public class UploadPhotoContentMCommandHandler : IRequestHandler<UploadPhotoContentMCommand, BasePhotoDto>
    {
        private readonly IBlobStorageService _blobStorageService;
        private readonly ISettingsService _settingsService;
        private readonly IPhotoConverterService _converterService;
        private readonly PhotoPackagesDbContext _dbContext;

        public UploadPhotoContentMCommandHandler(IBlobStorageService blobStorageService,
            PhotoPackagesDbContext dbContext, ISettingsService settingsService, IPhotoConverterService converterService)
        {
            _blobStorageService = blobStorageService;
            _dbContext = dbContext;
            _settingsService = settingsService;
            _converterService = converterService;
        }

        public async Task<BasePhotoDto> Handle(UploadPhotoContentMCommand request, CancellationToken cancellationToken)
        {
            var photo = await _dbContext.Photos.Include(p => p.PhotoPackage).FirstOrDefaultAsync(p =>
                                p.Id == request.PhotoId &&
                                p.PhotoPackage.Id == request.PackagePhotoId &&
                                p.PhotoPackage.ContractId == request.ContractId,
                            cancellationToken) ??
                        throw new ArgumentOutOfRangeException(nameof(request.PhotoId),
                            $"Фотография с идентификатором {request.PhotoId} не найдена");

            if (string.IsNullOrEmpty(photo.PhotoPackage.BucketId))
                throw new ArgumentOutOfRangeException(nameof(photo.Id),
                    $"Фотография с Id {photo.Id} не найдена в хранилище");
            
            if (photo.CanBeRetaken == false) throw new ArgumentOutOfRangeException(nameof(photo.Id),
                $"Фотография с Id {photo.Id} уже была загружена в хранилище");
            
            var contentId = await _blobStorageService.UploadPhoto(photo.PhotoPackage.BucketId, request.ContentName,
                request.SapNumber,
                request.ContentData,
                request.ContentType, cancellationToken);
            
            var thumbnailContent = _converterService.ConvertToThumbnail(request.ContentData);

            var thumbnailContentId = await _blobStorageService.UploadPhoto(photo.PhotoPackage.BucketId,
                request.ContentName, request.SapNumber,
                thumbnailContent,
                request.ContentType, cancellationToken);

            photo.ContentId = contentId;
            photo.ThumbnailContentId = thumbnailContentId;
            photo.StatusCode = PhotoStatuses.New;
            photo.ErrorCode = null;
            photo.ErrorMessage = null;
            photo.CanBeRetaken = false;
            
            await _dbContext.SaveChangesAsync(cancellationToken);

            return new BasePhotoDto
            {
                Id = photo.Id,
                Thumbnail = new PhotoImageDto
                {
                    Url = GetImageUri(CallerTypes.Mobile, photo.PhotoPackage.BucketId, photo.ThumbnailContentId)
                },
                Image = new PhotoImageDto
                {
                    FileName = request.ContentName,
                    ContentType = request.ContentType,
                    Url = GetImageUri(CallerTypes.Mobile, photo.PhotoPackage.BucketId, photo.ContentId)
                }
            };
        }

        private Uri GetImageUri(string callerType, string bucketId, string contentId)
        {
            var baseUri = _settingsService.GetBaseUriForContent(callerType);
            return new Uri($"{baseUri}/content/v1/{bucketId}/{contentId}/blob");
        }
    }
}