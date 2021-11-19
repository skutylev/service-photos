using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Service.Integrations.Client;
using Service.Integrations.Client.Contracts.ScanCheckerModels;
using Service.PhotoPackages.ServiceLayer.Integrations.Contracts;

namespace Service.PhotoPackages.ServiceLayer.Integrations
{
    public class ScanCheckerService : IScanCheckerService
    {
        private readonly IntegrationsServiceClient _integrationsServiceClient;

        public ScanCheckerService(IntegrationsServiceClient integrationsServiceClient)
        {
            _integrationsServiceClient = integrationsServiceClient;
        }

        public async Task<string> SendPhotos(SendPhotosRequest request, CancellationToken cancellationToken)
        {
            var response = await _integrationsServiceClient.SendPhotos(request, cancellationToken);
            await response.EnsureSuccessStatusCodeAsync();
            return response.Content.ScanCheckerRequestId;
        }

        public async Task<string> GetRequestId(string contractNumber, string processType,
            CancellationToken cancellationToken)
        {
            var response =
                await _integrationsServiceClient.GetScanCheckerId(contractNumber, processType, cancellationToken);
            
            await response.EnsureSuccessStatusCodeAsync();
            return response.Content.ScanCheckerRequestId;
        }

        public async Task<CheckPhotoStatusesResponse> CheckPhotoStatuses(IEnumerable<string> scanCheckerRequestIds,
            CancellationToken cancellationToken)
        {
            var response = await _integrationsServiceClient.CheckPhotoStatuses(new CheckPhotoStatusesRequest
            {
                ScanCheckerRequestIds = scanCheckerRequestIds
            }, cancellationToken);
            
            return response?.Content;
        }
    }
}