using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mbr.Dal;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Service.PhotoPackages.Dal;
using Service.PhotoPackages.Dal.Models;
using Service.PhotoPackages.ServiceLayer.Constants;
using Service.PhotoPackages.ServiceLayer.Integrations.Contracts;
using Service.PhotoPackages.ServiceLayer.Integrations.DMNs.PackagePhotoProcessTypes;
using Service.PhotoPackages.ServiceLayer.Integrations.DMNs.PackagePhotoTypesDmn;
using Service.PhotoPackages.ServiceLayer.Integrations.DMNs.PhotoTypesDmn;
using Service.PhotoPackages.ServiceLayer.MediatR.Commands.CreatePhotoPackage;
using Service.PhotoPackages.ServiceLayer.MediatR.Commands.UploadPhotoContent;
using Service.Photos.Dal;
using PackageType = Service.Photos.Dal.PackageType;
using PhotoPackage = Service.PhotoPackages.Dal.Models.PhotoPackage;


namespace Service.PhotoPackages.ServiceLayer.Migrator
{
    public static class ListExtensions
    {
        public static List<List<T>> ChunkBy<T>(this List<T> source, int chunkSize)
        {
            return source
                .Select((x, i) => new {Index = i, Value = x})
                .GroupBy(x => x.Index / chunkSize)
                .Select(x => x.Select(v => v.Value).ToList())
                .ToList();
        }

        public static IEnumerable<IEnumerable<T>> Split<T>(this IEnumerable<T> array, int size)
        {
            for (var i = 0; i < (float) array.Count() / size; i++)
            {
                yield return array.Skip(i * size).Take(size);
            }
        }
    }

    public class MigratorService : IMigratorService
    {
        private readonly MbrDbContext _mbrDbContext;
        private readonly PhotosContext _photosContext;
        private readonly PhotoPackagesDbContext _dbContext;
        private readonly IPhotoDmnService _photoDmnService;
        private readonly IMediator _mediator;

        public MigratorService(MbrDbContext mbrDbContext, IMediator mediator, PhotosContext photosContext,
            PhotoPackagesDbContext dbContext, IPhotoDmnService photoDmnService)
        {
            _mbrDbContext = mbrDbContext;
            _mediator = mediator;
            _photosContext = photosContext;
            _dbContext = dbContext;
            _photoDmnService = photoDmnService;
        }


        public async Task MigrateTravel(DateTime? startDate, DateTime? endDate)
        {
            startDate ??= DateTime.UtcNow.Date.AddMonths(-4);
            endDate ??= DateTime.UtcNow.Date;

            var existedPhotoPackageContractId = (await _dbContext
                .PhotoPackages
                .Where(p => p.TypeCode.Equals("APPLICATION_LK") || p.TypeCode.Equals("APPLICATION_VZR"))
                .Select(p => p.ContractId).AsNoTracking()
                .ToListAsync()).ToHashSet().Select(long.Parse);

            var commandsQuery = _mbrDbContext.Applications
                .Include(a => a.AdditionalServices)
                .Where(a => a.StartDate.Date >= startDate && a.StartDate.Date <= endDate)
                .Where(a => a.BucketId != null)
                .Where(a => a.ProductName.Contains("Travel"))
                .Where(a => existedPhotoPackageContractId.Contains(a.Id))
                .AsNoTracking()
                .Include(a => a.Client)
                .AsQueryable();

            var commands = await commandsQuery.Select(a => new CreatePhotoPackageMCommand
                {
                    ContractId = a.Id.ToString(),
                    BucketId = a.BucketId.ToString(),
                    ClientBirthday = a.Client.Birthday,
                    ProductTypeCode = a.ProductTypeCode,
                    ProductName = a.ProductName,
                    SigningStatus = a.SigningStatusCode,
                    AdditionalServicesCodes = a.AdditionalServices.Select(ass => ass.Code)
                })
                .ToListAsync();
            
            var newPhotoPackages = new List<PhotoPackage>();
            foreach (var command in commands)
            {
                newPhotoPackages.AddRange(MapPhotoPackage(command));
            }

            var existedPhotoPackageContractIdAsString = existedPhotoPackageContractId.Select(p => p.ToString());

            var existedPhotoPackages = await _dbContext
                .PhotoPackages
                .Include(p => p.Photos)    
                .Where(p => existedPhotoPackageContractIdAsString.Contains(p.ContractId) && p.StatusCode != PhotoPackageStatuses.Accepted)
                .ToListAsync();

            foreach (var newPhotoPackage in newPhotoPackages)
            {
                var existedPhotoPackage = existedPhotoPackages.FirstOrDefault(p =>
                    p.ContractId == newPhotoPackage.ContractId && p.TypeCode == newPhotoPackage.TypeCode);
                if (existedPhotoPackage == null) continue;
                foreach (var newPhoto in newPhotoPackage.Photos)
                {
                    var existedPhoto =
                        existedPhotoPackage.Photos.FirstOrDefault(p => p.TypeCode == newPhoto.TypeCode);
                    if (existedPhoto != null) continue;
                    existedPhotoPackage.Photos.Add(newPhoto);
                }
            }

            await _dbContext.SaveChangesAsync();
        }

        public async Task MigratePhotoPackages(List<long> packageIds, DateTime? startDate, DateTime? endDate)
        {
            startDate ??= DateTime.UtcNow.Date.AddMonths(-4);
            endDate ??= DateTime.UtcNow.Date;

            var commandsQuery = _mbrDbContext.Applications
                .Include(a => a.AdditionalServices)
                .Where(a => a.StartDate.Date >= startDate && a.StartDate.Date <= endDate)
                .Where(a => a.BucketId != null)
                .AsNoTracking()
                .Include(a => a.Client)
                .AsQueryable();

            if (packageIds.Any())
                commandsQuery.Where(p => packageIds.Contains(p.PackageId));

            var commands = await commandsQuery.Select(a => new CreatePhotoPackageMCommand
                {
                    ContractId = a.Id.ToString(),
                    BucketId = a.BucketId.ToString(),
                    ClientBirthday = a.Client.Birthday,
                    ProductTypeCode = a.ProductTypeCode,
                    ProductName = a.ProductName,
                    SigningStatus = a.SigningStatusCode,
                    AdditionalServicesCodes = a.AdditionalServices.Select(ass => ass.Code)
                })
                .ToListAsync();

            var createdContractIds = commands.Select(c => c.ContractId).ToHashSet();
            var existedPhotoPackagesContractIds =
                (await _dbContext.PhotoPackages.Select(p => p.ContractId).AsNoTracking().ToListAsync()).ToHashSet();
            createdContractIds.ExceptWith(existedPhotoPackagesContractIds);

            foreach (var chunk in commands.Where(c => createdContractIds.Contains(c.ContractId)).Split(10000))
            {
                var packages = new List<PhotoPackage>();
                foreach (var command in chunk)
                {
                    packages.AddRange(MapPhotoPackage(command));
                }

                await _dbContext.PhotoPackages.AddRangeAsync(packages);
                await _dbContext.SaveChangesAsync();
            }

            foreach (var splitCommands in commands.Split(1000))
            {
                var oldPhotoPackages = (await _photosContext.PhotoPackages
                    .Include(pp => pp.ApplicationPhotos)
                    .ThenInclude(ap => ap.Attachment)
                    .Where(pp => splitCommands.Select(c => long.Parse(c.ContractId)).Contains(pp.ApplicationId))
                    .AsNoTracking()
                    .Select(a => new
                    {
                        a.PackageStatus,
                        a.ApplicationId,
                        a.AproveDate,
                        a.ErrorDate,
                        a.ErrorMessage,
                        a.ErrorText,
                        a.ErrorFixDate,
                        a.Type,
                        a.RequestId,
                        a.UserSapNumber,
                        a.SendPackageDate,
                        ApplicationPhotos = a.ApplicationPhotos.Select(ap => new
                        {
                            ap.Latitude,
                            ap.Longitude,
                            ap.AttachmentId,
                            ap.ErrorCode,
                            ap.ErrorText,
                            ap.FoundDate,
                            ap.PhotoTypeId,
                            ap.ParentPhotoTypeId,
                        })
                    }).ToListAsync());
                
                foreach (var oldPhotoPackage in oldPhotoPackages)
                {
                    var command =
                        commands.FirstOrDefault(c => c.ContractId == oldPhotoPackage.ApplicationId.ToString());

                    var packageTypeCode = MapPackageTypeCodeFromType(oldPhotoPackage.Type);
                    if (string.IsNullOrEmpty(packageTypeCode))
                        continue;

                    var newPhotoPackage = await _dbContext.PhotoPackages.Include(pp => pp.Photos).FirstOrDefaultAsync(
                        p =>
                            p.ContractId == oldPhotoPackage.ApplicationId.ToString() &&
                            p.TypeCode == packageTypeCode &&
                            p.StatusCode == PhotoPackageStatuses.New
                    );
                    
                    if (newPhotoPackage == null)
                        continue;

                    var latitude = default(double?);
                    var longitude = default(double?);

                    foreach (var oldPhoto in oldPhotoPackage.ApplicationPhotos)
                    {
                        latitude ??= oldPhoto.Latitude;
                        longitude ??= oldPhoto.Longitude;

                        var photoTypeCode = MapPhotoTypeCodeFromId(oldPhoto.PhotoTypeId);
                        if (string.IsNullOrEmpty(photoTypeCode))
                            continue;

                        var newPhoto = newPhotoPackage.Photos.FirstOrDefault(p => p.TypeCode == photoTypeCode && p.ContentId == null);

                        if (newPhoto == null)
                            continue;

                        var attachment = await _photosContext.Attachments.AsNoTracking()
                            .FirstOrDefaultAsync(a => a.Id == oldPhoto.AttachmentId);

                        await _mediator.Send(new UploadPhotoContentMCommand
                        {
                            ContractId = oldPhotoPackage.ApplicationId.ToString(),
                            SapNumber = oldPhotoPackage.UserSapNumber,
                            PhotoId = newPhoto.Id,
                            PackagePhotoId = newPhoto.PhotoPackageId,
                            ContentData = attachment.Photo,
                            ContentName = $"{newPhoto.Id}.jpg",
                            ContentType = "image/jpeg"
                        });
                    }

                    var processCode = _photoDmnService.GetPackagePhotoProcessTypes(
                        new PackagePhotoProcessTypesDmnRequest
                        {
                            CourierHasOnlineActivationRight = oldPhotoPackage.RequestId.HasValue,
                            ProductTypeCode = command.ProductTypeCode,
                            SigningStatus = command.SigningStatus
                        }).ProcessTypeCode;

                    newPhotoPackage.StatusCode = PhotoPackageStatuses.Saved;
                    newPhotoPackage.ProcessCode = processCode;
                    newPhotoPackage.Latitude = latitude;
                    newPhotoPackage.Longitude = longitude;
                    newPhotoPackage.AuthorSapNumber = oldPhotoPackage.UserSapNumber;

                    await _dbContext.SaveChangesAsync();

                    newPhotoPackage.StatusCode = PhotoPackageStatuses.Pending;
                    if (oldPhotoPackage.RequestId.HasValue)
                        newPhotoPackage.ScanCheckerRequestId = oldPhotoPackage.RequestId.Value.ToString();

                    await _dbContext.SaveChangesAsync();

                    switch (oldPhotoPackage.PackageStatus)
                    {
                        case PhotoPackageStatus.Accepted:
                            newPhotoPackage.StatusCode = PhotoPackageStatuses.Accepted;
                            foreach (var photo in newPhotoPackage.Photos)
                            {
                                var oldPhoto = oldPhotoPackage.ApplicationPhotos.FirstOrDefault(p =>
                                    p.PhotoTypeId == MapPhotoTypeIdFromCode(photo.TypeCode));
                                if (oldPhoto == null) continue;
                                photo.ErrorCode = oldPhoto.ErrorCode;
                                photo.ErrorMessage = oldPhoto.ErrorText;
                                photo.StatusCode = PhotoStatuses.Accepted;
                        
                            }
                            await _dbContext.SaveChangesAsync();
                            break;
                        case PhotoPackageStatus.Rejected:
                            newPhotoPackage.StatusCode = PhotoPackageStatuses.Rejected;
                            newPhotoPackage.ErrorCode = !string.IsNullOrEmpty(oldPhotoPackage.ErrorMessage)
                                ? oldPhotoPackage.ErrorMessage
                                : (string.IsNullOrEmpty(oldPhotoPackage.ErrorMessage) &&
                                   !string.IsNullOrEmpty(oldPhotoPackage.ErrorText)
                                    ? "REJECT_UVIA"
                                    : null);
                            newPhotoPackage.ErrorMessage = oldPhotoPackage.ErrorText;

                            foreach (var photo in newPhotoPackage.Photos)
                            {
                                var oldPhoto = oldPhotoPackage.ApplicationPhotos.FirstOrDefault(p =>
                                    p.PhotoTypeId == MapPhotoTypeIdFromCode(photo.TypeCode));
                                if (oldPhoto == null) continue;
                                photo.StatusCode = PhotoStatuses.Rejected;
                                photo.ErrorCode = oldPhoto.ErrorCode;
                                photo.ErrorMessage = oldPhoto.ErrorText;
                         
                            }
                            await _dbContext.SaveChangesAsync();
                            break;
                    }
                }
            }
        }

        private List<PhotoPackage> MapPhotoPackage(CreatePhotoPackageMCommand request)
        {
            var packagePhotoTypes = _photoDmnService.GetPackagePhotoTypes(new PackagePhotoTypesDmnRequest
            {
                ProductName = request.ProductName,
                ProductTypeCode = request.ProductTypeCode,
                AdditionalServiceLk =
                    request.AdditionalServicesCodes.Contains("LK", StringComparer.InvariantCultureIgnoreCase),
                AdditionalServiceVzr =
                    request.AdditionalServicesCodes.Contains("VZR", StringComparer.InvariantCultureIgnoreCase)
            });

            var photoTypes = packagePhotoTypes.Select(p => new
            {
                Code = p.PackagePhotoTypeCode,
                PhotoTypes = _photoDmnService.GetPhotoTypes(new PhotoTypesDmnRequest
                {
                    ProductName = request.ProductName,
                    ProductTypeCode = request.ProductTypeCode,
                    SigningStatus = request.SigningStatus,
                    PackagePhotoTypeCode = p.PackagePhotoTypeCode,
                    IsUnderage = IsUnderage(request.ClientBirthday)
                })
            }).ToDictionary(k => k.Code, v => v.PhotoTypes);

            return photoTypes.Select(p => new PhotoPackage
            {
                StatusCode = PhotoPackageStatuses.New,
                ContractId = request.ContractId,
                TypeCode = p.Key,
                BucketId = request.BucketId,
                ClientBirthday = request.ClientBirthday,
                Photos = p.Value.Select(pp => new Photo
                {
                    TypeCode = pp.PhotoTypeCode,
                    Required = pp.Required,
                    AttachSystemCode = AttachSystems.MobileBanker,
                    CanBeRetaken = true,
                    StatusCode = PhotoStatuses.New
                }).ToList()
            }).ToList();
        }

        private static bool IsUnderage(DateTime clientBirthday, DateTime? checkDate = default)
        {
            checkDate ??= DateTime.UtcNow;
            var age = checkDate.Value.Year - clientBirthday.Year;
            if (clientBirthday > checkDate.Value.AddYears(-age)) age--;
            return age < 18;
        }

        private string MapPhotoTypeCodeFromId(int photoTypeId) =>
            photoTypeId switch
            {
                1 => "CLIENT_FOTO",
                3 => "PASSPORT_FOTO",
                4 => "PASSPORT_REGISTRATION",
                5 => "PASSPORT_PREVIOUS_INFO",
                6 => "SNILS",
                9 => "IU_LIST",
                12 => "CLIENT_WITH_KMW",
                13 => "CLIENT_FOTO_WITH_CARD",
                17 => "CARD_DELIVERY",
                21 => "APPLICATION_FIRST",
                22 => "APPLICATION_LAST",
                23 => "PERS_DATA_AGREEMENT",
                24 => "REFUSAL_OF_RIGHTS",
                27 => "BIRTH_CERTIFICATE",
                5872 => "LK",
                5873 => "VZR",
                _ => string.Empty
            };

        private int MapPhotoTypeIdFromCode(string photoTypeCode) =>
            photoTypeCode switch
            {
                "CLIENT_FOTO" => 1,
                "PASSPORT_FOTO" => 3,
                "PASSPORT_REGISTRATION" => 4,
                "PASSPORT_PREVIOUS_INFO" => 5,
                "SNILS" => 6,
                "IU_LIST" => 9,
                "CLIENT_WITH_KMW" => 12,
                "CLIENT_FOTO_WITH_CARD" => 13,
                "CARD_DELIVERY" => 17,
                "APPLICATION_FIRST" => 21,
                "APPLICATION_LAST" => 22,
                "PERS_DATA_AGREEMENT" => 23,
                "REFUSAL_OF_RIGHTS" => 24,
                "BIRTH_CERTIFICATE" => 27,
                "LK" => 5872,
                "VZR" => 5873,
                _ => 0
            };

        private string MapPackageTypeCodeFromType(PackageType type) =>
            type switch
            {
                PackageType.Contract => "CONTRACT",
                PackageType.LoungeKey => "APPLICATION_LK",
                PackageType.TravelInsurance => "APPLICATION_VZR",
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
            };
    }

    public interface IMigratorService
    {
        Task MigratePhotoPackages(List<long> packageIds, DateTime? startDate, DateTime? endDate);
        Task MigrateTravel(DateTime? startDate, DateTime? endDate);
    }
}