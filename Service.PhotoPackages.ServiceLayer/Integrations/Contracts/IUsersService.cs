using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Service.Users.Client.Contracts;
using Executor = Service.PhotoPackages.Client.Contracts.Executor;

namespace Service.PhotoPackages.ServiceLayer.Integrations.Contracts
{
    public interface IUsersService
    {
        Task<List<Executor>> GetExecutors(long[] sapNumbers, CancellationToken cancellationToken);
        Task<List<UserModel>> GetUsers(long[] sapNumbers, CancellationToken cancellationToken);
    }
}