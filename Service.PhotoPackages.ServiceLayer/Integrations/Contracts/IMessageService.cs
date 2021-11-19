using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.PhotoPackages.ServiceLayer.Integrations.Contracts
{
    public interface IMessageService
    {
        Task SendEmail(IEnumerable<string> receivers, string subject, string body);
        Task SendPush(string templateCode, Dictionary<string, object> parameters, long sapNumber);
    }
}