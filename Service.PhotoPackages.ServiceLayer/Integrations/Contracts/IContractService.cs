using System.Threading;
using System.Threading.Tasks;
using Service.Contracts.Client.Contracts.Contracts;

namespace Service.PhotoPackages.ServiceLayer.Integrations.Contracts
{
    public interface IContractService
    {
        Task<ContractItemDto> GetContractInfo(string contractId, CancellationToken cancellationToken);
    }
}