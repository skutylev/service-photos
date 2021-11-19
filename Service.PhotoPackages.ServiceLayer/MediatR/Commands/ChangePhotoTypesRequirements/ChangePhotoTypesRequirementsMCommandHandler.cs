using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Service.PhotoPackages.Dal;
using Service.PhotoPackages.Dal.Models;

namespace Service.PhotoPackages.ServiceLayer.MediatR.Commands.ChangePhotoTypesRequirements
{
    public class ChangePhotoTypesRequirementsMCommandHandler : AsyncRequestHandler<ChangePhotoTypesRequirementsMCommand>
    {
        private readonly PhotoPackagesDbContext _dbContext;

        public ChangePhotoTypesRequirementsMCommandHandler(PhotoPackagesDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        protected override async Task Handle(ChangePhotoTypesRequirementsMCommand request,
            CancellationToken cancellationToken)
        {
            var photoPackage = await GetPhotoPackage(request.ContractId, request.PhotoPackageId, cancellationToken);

            foreach (var photo in photoPackage.Photos.Where(p =>
                request.PhotoRequirements.Select(pr => pr.photoId).Contains(p.Id)))
            {
                var photoRequirement = request.PhotoRequirements.FirstOrDefault(pr => pr.photoId == photo.Id);
                photo.Required = photoRequirement.required;
            }

            await _dbContext.SaveChangesAsync(cancellationToken);
        }
        
        
        private async Task<PhotoPackage> GetPhotoPackage(string contractId, string packagePhotoId,
            CancellationToken cancellationToken)
        {
            return await _dbContext.PhotoPackages
                       .Include(p => p.Photos).IgnoreAutoIncludes()
                       .FirstOrDefaultAsync(pp =>
                           pp.ContractId == contractId && pp.Id == packagePhotoId, cancellationToken) ??
                   throw new ArgumentOutOfRangeException(nameof(packagePhotoId),
                       "Пакет фото с указанным PackagePhotoId не найден");
        }
    }
}