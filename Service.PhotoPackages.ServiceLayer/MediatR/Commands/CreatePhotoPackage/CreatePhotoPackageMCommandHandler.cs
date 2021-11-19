using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MassTransit;
using MediatR;
using Service.PhotoPackages.Dal;
using Service.PhotoPackages.Dal.Models;
using Service.PhotoPackages.Events;
using Service.PhotoPackages.ServiceLayer.Constants;
using Service.PhotoPackages.ServiceLayer.Integrations.Contracts;
using Service.PhotoPackages.ServiceLayer.Integrations.DMNs.PackagePhotoTypesDmn;
using Service.PhotoPackages.ServiceLayer.Integrations.DMNs.PhotoTypesDmn;

namespace Service.PhotoPackages.ServiceLayer.MediatR.Commands.CreatePhotoPackage
{
    public class CreatePhotoPackageMCommandHandler: AsyncRequestHandler<CreatePhotoPackageMCommand>
    {
        private readonly PhotoPackagesDbContext _dbContext;
        private readonly IPhotoDmnService _photoDmnService;
        private readonly IBus _bus;

        public CreatePhotoPackageMCommandHandler(PhotoPackagesDbContext dbContext, IPhotoDmnService photoDmnService, IBus bus)
        {
            _dbContext = dbContext;
            _photoDmnService = photoDmnService;
            _bus = bus;
        }

        protected override async Task Handle(CreatePhotoPackageMCommand request, CancellationToken cancellationToken)
        {
            if (_dbContext.PhotoPackages.Any(p => p.ContractId == request.ContractId)) 
                return;

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
            
            var photoPackages = photoTypes.Select(p => new PhotoPackage
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
            
            await _dbContext.PhotoPackages.AddRangeAsync(photoPackages, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);

            foreach (var package in photoPackages)
            {
               await _bus.Publish(new PhotoPackageCreatedEvent
               {
                   Id = package.Id,
                   ContractId = package.ContractId,
                   TypeCode = package.TypeCode
               }, cancellationToken);
            }
        }
        
        private static bool IsUnderage(DateTime clientBirthday, DateTime? checkDate = default)
        {
            checkDate ??= DateTime.UtcNow;
            var age = checkDate.Value.Year - clientBirthday.Year;
            if (clientBirthday > checkDate.Value.AddYears(-age)) age--;
            return age < 18;
        }
    }
}