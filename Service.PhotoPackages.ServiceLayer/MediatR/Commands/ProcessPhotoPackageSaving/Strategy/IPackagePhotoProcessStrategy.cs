using System.Threading;
using System.Threading.Tasks;
using Service.PhotoPackages.Dal.Models;

namespace Service.PhotoPackages.ServiceLayer.MediatR.Commands.ProcessPhotoPackageSaving.Strategy
{
    public interface IPackagePhotoProcessStrategy
    {
        Task<ProcessResult> Apply(PhotoPackage photoPackage, CancellationToken cancellationToken);
    }
}