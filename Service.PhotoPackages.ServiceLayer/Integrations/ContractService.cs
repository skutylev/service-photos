using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Service.Contracts.Client;
using Service.Contracts.Client.Contracts.Contracts;
using Service.PhotoPackages.ServiceLayer.Integrations.Contracts;

namespace Service.PhotoPackages.ServiceLayer.Integrations
{
    public class ContractService : IContractService
    {
        private readonly IMemoryCache _memoryCache;
        private readonly ServiceContractsClient _serviceContractsClient;

        public ContractService(IMemoryCache memoryCache, ServiceContractsClient serviceContractsClient)
        {
            _memoryCache = memoryCache;
            _serviceContractsClient = serviceContractsClient;
        }

        public async Task<ContractItemDto> GetContractInfo(string contractId, CancellationToken cancellationToken)
        {
            if (_memoryCache.TryGetValue(contractId, out ContractItemDto contractInfo) && contractInfo != null) 
                return contractInfo;
            var contractInfoResponse = await _serviceContractsClient.GetContractUvia(contractId, cancellationToken);
            if (!contractInfoResponse.IsSuccessStatusCode || contractInfoResponse.Content == null) return contractInfo;
            
            _memoryCache.Set(contractId, contractInfoResponse.Content,
                new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromDays(1)));

            return contractInfoResponse.Content;
        }
    }
}