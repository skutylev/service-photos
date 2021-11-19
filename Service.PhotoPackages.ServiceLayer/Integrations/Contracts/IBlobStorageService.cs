using System.Threading;
using System.Threading.Tasks;

namespace Service.PhotoPackages.ServiceLayer.Integrations.Contracts
{
    public interface IBlobStorageService
    {
        Task<string> UploadPhoto(string bucketId, string contentName, long sapNumber, byte[] contentData, string contentType, CancellationToken cancellationToken);

        Task<byte[]> GetPhoto(string bucketId, string contentId, CancellationToken cancellationToken);
    }
}