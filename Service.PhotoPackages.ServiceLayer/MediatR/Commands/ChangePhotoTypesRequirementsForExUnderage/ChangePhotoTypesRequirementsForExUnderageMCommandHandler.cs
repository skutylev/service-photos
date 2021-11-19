using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Service.PhotoPackages.Dal;
using Service.PhotoPackages.Dal.Models;
using Service.PhotoPackages.ServiceLayer.MediatR.Commands.ChangePhotoTypesRequirements;

namespace Service.PhotoPackages.ServiceLayer.MediatR.Commands.ChangePhotoTypesRequirementsForExUnderage
{
    public class ChangePhotoTypesRequirementsForExUnderageMCommandHandler : AsyncRequestHandler<ChangePhotoTypesRequirementsForExUnderageMCommand>
    {
        private readonly PhotoPackagesDbContext _dbContext;
        private readonly IMediator _mediator;
        private const string ParentPrefix = "PARENT_";

        public ChangePhotoTypesRequirementsForExUnderageMCommandHandler(PhotoPackagesDbContext dbContext, IMediator mediator)
        {
            _dbContext = dbContext;
            _mediator = mediator;
        }

        protected override async Task Handle(ChangePhotoTypesRequirementsForExUnderageMCommand request,
            CancellationToken cancellationToken)
        {
            var photoPackages = await GetPhotoPackages(cancellationToken);

            foreach (var photoPackage in photoPackages)
            {
                var changingPhotos = photoPackage.Photos.Where(p => p.TypeCode.Contains(ParentPrefix)).ToList();
                if (!changingPhotos.Any()) continue;
                
                await _mediator.Send(new ChangePhotoTypesRequirementsMCommand
                {
                    ContractId = photoPackage.ContractId,
                    PhotoPackageId = photoPackage.Id,
                    PhotoRequirements = changingPhotos.Select(p => (p.Id, false))
                }, cancellationToken);
            }
        }
        
        private async Task<IEnumerable<PhotoPackage>> GetPhotoPackages(
            CancellationToken cancellationToken)
        {
            var date = DateTime.Now.Date.AddYears(-18);
            return await _dbContext.PhotoPackages
                .Include(p => p.Photos)
                .IgnoreAutoIncludes()
                .Where(pp => 
                    pp.ClientBirthday == date && 
                    pp.Photos.Any(p => p.TypeCode.Contains(ParentPrefix)) &&
                    (pp.StatusCode == PhotoPackageStatuses.New || pp.StatusCode == PhotoPackageStatuses.Rejected))
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }
    }
}