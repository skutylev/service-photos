using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Service.Integrations.Client.Contracts.ScanCheckerModels;

namespace Service.PhotoPackages.ServiceLayer.Integrations.Contracts
{
    public interface IScanCheckerService
    {
        Task<string> SendPhotos(SendPhotosRequest request, CancellationToken cancellationToken);

        Task<CheckPhotoStatusesResponse> CheckPhotoStatuses(IEnumerable<string> scanCheckerRequestIds,
            CancellationToken cancellationToken);

        Task<string> GetRequestId(string contractNumber, string processType, CancellationToken cancellationToken);
    }
}