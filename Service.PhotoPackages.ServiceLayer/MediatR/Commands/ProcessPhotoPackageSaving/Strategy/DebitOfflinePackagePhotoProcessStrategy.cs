using System.Threading;
using System.Threading.Tasks;
using Service.PhotoPackages.Dal;
using Service.PhotoPackages.Dal.Models;

namespace Service.PhotoPackages.ServiceLayer.MediatR.Commands.ProcessPhotoPackageSaving.Strategy
{
    public class DebitOfflinePackagePhotoProcessStrategy : IPackagePhotoProcessStrategy
    {
        public async Task<ProcessResult> Apply(PhotoPackage photoPackage, CancellationToken cancellationToken)
        {
            await Task.CompletedTask;
            return new ProcessResult
            {
                PhotoPackageStatusCode = PhotoPackageStatuses.Accepted,
            };
        }
    }
}