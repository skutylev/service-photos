using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Service.PhotoPackages.Client.Contracts;
using Service.PhotoPackages.Dal;
using Service.PhotoPackages.Dal.Models;
using Service.PhotoPackages.ServiceLayer.Constants;
using Service.PhotoPackages.ServiceLayer.Integrations.Contracts;
using Service.Settings.Client.Contracts.Dto;

namespace Service.PhotoPackages.ServiceLayer.MediatR.Requests.GetPhotoPackage
{
    public class GetPhotoPackageMRequestHandler : IRequestHandler<GetPhotoPackageMRequest, IEnumerable<PhotoPackageDto>>
    {
        private readonly PhotoPackagesDbContext _dbContext;
        private readonly IUsersService _usersService;
        private readonly ISettingsService _settingsService;

        public GetPhotoPackageMRequestHandler(PhotoPackagesDbContext dbContext,
            ISettingsService settingsService, IUsersService usersService)
        {
            _dbContext = dbContext;
            _settingsService = settingsService;
            _usersService = usersService;
        }

        public async Task<IEnumerable<PhotoPackageDto>> Handle(GetPhotoPackageMRequest request,
            CancellationToken cancellationToken)
        {
            var query = _dbContext.PhotoPackages
                .Where(pp => pp.ContractId == request.ContractId)
                .Include(pp => pp.Photos)
                .AsQueryable();

            if (request.IncludePhotoPackageHistory)
                query = query.Include(pp => pp.History);

            if (request.IncludePhotoHistory)
                query = query.Include(pp => pp.Photos)
                    .ThenInclude(ph => ph.History);

            var photoPackages = await query.AsNoTracking()
                .ToListAsync(cancellationToken);

            if (!photoPackages.Any()) return new List<PhotoPackageDto>();

            var sapNumbers = photoPackages
                .Select(a => (a.AuthorSapNumber ?? 0, a.VerifierSapNumber ?? 0))
                .Aggregate<(long, long), ICollection<long>>(new List<long>(), (sn, snTuple) =>
                {
                    sn.Add(snTuple.Item1);
                    sn.Add(snTuple.Item2);
                    return sn;
                }).Where(s => s != 0)
                .ToHashSet().Union(photoPackages.Where(p => p.History != null && p.History.Any())
                    .SelectMany(a => a.History?.Select(s => (s.AuthorSapNumber ?? 0 , s.VerifierSapNumber ?? 0)))
                    .Aggregate<(long, long), ICollection<long>>(new List<long>(), (sn, snTuple) =>
                    {
                        sn.Add(snTuple.Item1);
                        sn.Add(snTuple.Item2);
                        return sn;
                    }).Where(s => s != 0)
                    .ToHashSet()).ToList();
            
            var users = sapNumbers.Any() ? await _usersService.GetExecutors(sapNumbers.ToArray(), cancellationToken) : new List<Executor>();

            return photoPackages.Select(p => MapPhotoPackageToPhotoPackageDto(p, users, request.CallerType));
        }

        private PhotoPackageDto MapPhotoPackageToPhotoPackageDto(PhotoPackage package, List<Executor> users, string callerType)
        {
            var dto = new PhotoPackageDto
            {
                Id = package.Id,
                Type = _settingsService.GetPhotoPackageTypes().FirstOrDefault(t => t.Code == package.TypeCode),
                Status = _settingsService.GetPhotoPackageStatuses().FirstOrDefault(t => t.Code == package.StatusCode),
                Coordinates = package.Latitude.HasValue && package.Longitude.HasValue
                    ? new Coordinates {Latitude = package.Latitude.Value, Longitude = package.Longitude.Value}
                    : null,
                Author = package.AuthorSapNumber.HasValue
                    ? users.FirstOrDefault(u => u.SapNumber == package.AuthorSapNumber)
                    : null,
                Verifier = package.VerifierSapNumber.HasValue
                    ? users.FirstOrDefault(u => u.SapNumber == package.VerifierSapNumber)
                    : null,
                Error = !string.IsNullOrEmpty(package.ErrorCode) || !string.IsNullOrEmpty(package.ErrorMessage)
                    ? new DictionaryItemDto
                    {
                        Code = package.ErrorCode,
                        Name = package.ErrorMessage,
                        Flag = 0
                    }
                    : null,
                StartDate = _settingsService.GetDateTimeOffsetFromUtcTime(package.StartDate, 0),
                UpdateDate = package.UpdateDate.HasValue
                    ? _settingsService.GetDateTimeOffsetFromUtcTime(package.UpdateDate.Value, 0)
                    : null,
                UploadUrl = GetMobileUploadPackagePhotoUri(callerType, package.ContractId, package.StatusCode, package.Id)
            };

            if (package.History != null && package.History.Any())
                dto.History = package.History.Select(s => MapPhotoPackageHistoryToPhotoPackageHistoryItem(s, users));

            if (package.Photos != null && package.Photos.Any())
                dto.Photos = package.Photos.Select(s => MapPhotoDtoFromPhoto(s, callerType)).OrderBy(p => p.Group.Flag).ThenBy(p => p.Type.Flag);
 
            return dto;
        }

        private PhotoPackageHistoryItemDto MapPhotoPackageHistoryToPhotoPackageHistoryItem(
            PhotoPackageHistory historyItem,
            List<Executor> users)
        {
            return new()
            {
                ChangeId = historyItem.Id.ToString(),
                Status = _settingsService.GetPhotoPackageStatuses()
                    .FirstOrDefault(t => t.Code == historyItem.StatusCode),
                Author = historyItem.AuthorSapNumber.HasValue
                    ? users.FirstOrDefault(u => u.SapNumber == historyItem.AuthorSapNumber)
                    : null,
                Verifier = historyItem.VerifierSapNumber.HasValue
                    ? users.FirstOrDefault(u => u.SapNumber == historyItem.VerifierSapNumber)
                    : null,
                Error = !string.IsNullOrEmpty(historyItem.ErrorCode) || !string.IsNullOrEmpty(historyItem.ErrorMessage)
                    ? new DictionaryItemDto
                    {
                        Code = historyItem.ErrorCode,
                        Name = historyItem.ErrorMessage,
                        Flag = 0
                    }
                    : null,
                Coordinates = historyItem.Latitude.HasValue && historyItem.Longitude.HasValue
                    ? new Coordinates {Latitude = historyItem.Latitude.Value, Longitude = historyItem.Longitude.Value}
                    : null,
                StartDate = _settingsService.GetDateTimeOffsetFromUtcTime(historyItem.StartDate, 0),
            };
        }

        private PhotoDto MapPhotoDtoFromPhoto(Photo photo, string callerType)
        {
            var dto = new PhotoDto
            {
                Id = photo.Id,
                Type = _settingsService.GetPhotoTypes(callerType).FirstOrDefault(t => t.Code == photo.TypeCode),
                Group = _settingsService.GetPhotoGroupType(photo.TypeCode),
                Status = !string.IsNullOrEmpty(photo.StatusCode) ? _settingsService.GetPhotoStatuses().FirstOrDefault(t => t.Code == photo.StatusCode) : null,
                Required = photo.Required,
                Error = !string.IsNullOrEmpty(photo.ErrorCode) || !string.IsNullOrEmpty(photo.ErrorMessage)
                    ? new DictionaryItemDto
                    {
                        Code = photo.ErrorCode,
                        Name = photo.ErrorMessage,
                        Flag = 0
                    }
                    : null,
                CanBeRetaken = photo.CanBeRetaken,
                StartDate = _settingsService.GetDateTimeOffsetFromUtcTime(photo.StartDate, 0),
                UpdateDate = photo.UpdateDate.HasValue
                    ? _settingsService.GetDateTimeOffsetFromUtcTime(photo.UpdateDate.Value, 0)
                    : null,
                Image =  new PhotoImageDto
                {
                    Url = !string.IsNullOrEmpty(photo.ContentId) ? GetImageUri(callerType, photo.PhotoPackage.BucketId, photo.ContentId) : null ,
                    UploadUrl = photo.CanBeRetaken ? GetMobileUploadPhotoUri(callerType, photo.PhotoPackage.ContractId, photo.PhotoPackage.StatusCode, photo.PhotoPackage.Id, photo.Id) : null
                },
                Thumbnail =  !string.IsNullOrEmpty(photo.ThumbnailContentId) ? new PhotoImageDto { Url = GetImageUri(callerType, photo.PhotoPackage.BucketId, photo.ThumbnailContentId) } : null
                    
            };

            if (photo.History != null && photo.History.Any())
                dto.History = photo.History.Select(s => MapPhotoDtoFromPhotoHistory(s, callerType));

            return dto;
        }

        private PhotoHistoryItemDto MapPhotoDtoFromPhotoHistory(PhotoHistory historyItem, string callerType)
        {
            return new()
            {
                ChangeId = historyItem.Id,
                Error = !string.IsNullOrEmpty(historyItem.ErrorCode) || !string.IsNullOrEmpty(historyItem.ErrorMessage)
                    ? new DictionaryItemDto
                    {
                        Code = historyItem.ErrorCode,
                        Name = historyItem.ErrorMessage,
                        Flag = 0
                    }
                    : null,
                Status = !string.IsNullOrEmpty(historyItem.StatusCode) ? _settingsService.GetPhotoStatuses().FirstOrDefault(t => t.Code == historyItem.StatusCode) : null,
                StartDate = _settingsService.GetDateTimeOffsetFromUtcTime(historyItem.StartDate, 0),
                Image = !string.IsNullOrEmpty(historyItem.ContentId) ? new PhotoImageDto { Url = GetImageUri(callerType, historyItem.Photo.PhotoPackage.BucketId, historyItem.ContentId) } : null,
                Thumbnail = !string.IsNullOrEmpty(historyItem.ThumbnailContentId) ? new PhotoImageDto { Url = GetImageUri(callerType, historyItem.Photo.PhotoPackage.BucketId, historyItem.ThumbnailContentId) } : null,
            };
        }

        private Uri GetImageUri(string callerType, string bucketId, string contentId)
        {
            var baseUri = _settingsService.GetBaseUriForContent(callerType);
            return new Uri($"{baseUri}/content/v1/{bucketId}/{contentId}/blob");
        }

        private Uri GetMobileUploadPhotoUri(string callerType, string contractId, string packagePhotoStatusCode, string photoPackageId, string photoId)
        {
            switch (callerType)
            {
                case CallerTypes.Mobile when new List<string> {PhotoPackageStatuses.New, PhotoPackageStatuses.Rejected}.Contains(packagePhotoStatusCode):
                    var baseUri = _settingsService.GetBaseUriForContent(CallerTypes.Mobile);
                    return new Uri($"{baseUri}/photos/v2/{contractId}/{photoPackageId}/{photoId}");
                default:
                    return null;
            }
        }

        private Uri GetMobileUploadPackagePhotoUri(string callerType, string contractId, string packagePhotoStatusCode, string photoPackageId)
        {
            switch (callerType)
            {
                case CallerTypes.Mobile when new List<string> {PhotoPackageStatuses.New, PhotoPackageStatuses.Rejected}.Contains(packagePhotoStatusCode):
                    var baseUri = _settingsService.GetBaseUriForContent(CallerTypes.Mobile);
                    return new Uri($"{baseUri}/photos/v2/{contractId}/{photoPackageId}");
                default:
                    return null;
            }
        }
    }
}