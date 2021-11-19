using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Mbr.ExpressionUtils;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Service.PhotoPackages.Dal;
using Service.PhotoPackages.Dal.Models;

namespace Service.PhotoPackages.ServiceLayer.MediatR.Commands.UpdateScanServerDataMCommand
{
    public class UpdateScanServerDataMCommandHandler : AsyncRequestHandler<UpdateScanServerDataMCommand>
    {
        private readonly PhotoPackagesDbContext _dbContext;

        public UpdateScanServerDataMCommandHandler(PhotoPackagesDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        protected override async Task Handle(UpdateScanServerDataMCommand request, CancellationToken cancellationToken)
        {
            var changedPhotos = await _dbContext.Photos.Where(FilterChanged(request.PhotoInfos))
                .ToListAsync(cancellationToken: cancellationToken);
            
            foreach (var changedPhoto in changedPhotos)
            {
                var scanServerInfo = request.PhotoInfos.FirstOrDefault(p => p.PhotoId == changedPhoto.Id);
                if (scanServerInfo == null) continue;

                changedPhoto.ScanServerDocumentId = scanServerInfo.ScanServerDocumentId;
                changedPhoto.ScanServerPageId = scanServerInfo.ScanServerPageId;
                changedPhoto.ScanServerPageNumber = scanServerInfo.ScanServerPageNumber;
                changedPhoto.TranzWareScanId = scanServerInfo.TwScanDocumentId;
            }

            await _dbContext.SaveChangesAsync(cancellationToken);
        }


        private static Expression<Func<Photo, bool>> FilterChanged(
            IEnumerable<UpdateScanServerDataMCommand.PhotoScanServerInfo> infos)
        {
            Expression<Func<Photo, bool>> result = default;
            
            foreach (var info in infos)
            {
                Expression<Func<Photo, bool>> infoResult = r => 
                    info.PhotoId == r.Id && 
                    (info.ScanServerPageId != r.ScanServerPageId || info.ScanServerDocumentId != r.ScanServerDocumentId || info.TwScanDocumentId != r.TranzWareScanId) ;
                result = result.Or(infoResult);
            }

            return result;
        }

    }
}