using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Mbr.QueueProvider;
using Microsoft.Extensions.DependencyInjection;
using Service.Messages.Client.Contracts.QueueMessages;
using Service.PhotoPackages.ServiceLayer.Integrations.Contracts;

namespace Service.PhotoPackages.ServiceLayer.Integrations
{
    public class MessageService : IMessageService
    {
        private readonly IServiceProvider _serviceProvider;

        public MessageService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task SendEmail(IEnumerable<string> receivers, string subject, string body)
        {
            var message = new EmailQueueMessage
            {
                Body = body,
                Receivers = receivers,
                Subject = subject,
                HtmlBody = true,
            };
            
            using var queuePublisher = _serviceProvider.GetRequiredService<IQueuePublisher>();
            queuePublisher.Publish(message);
            await Task.CompletedTask;
        }

        public async Task SendPush(string templateCode, Dictionary<string, object> parameters, long sapNumber)
        {
            parameters.TryGetValue("ContractNumber", out var contractNumber);
            parameters.TryGetValue("ContractId", out var contractId);
            parameters.TryGetValue("PackageId", out var packageId);

            var message = new PushQueueMessage
            {
                Message = new PushPayload
                {
                    TemplateCode = templateCode,
                    Parameters = new List<object>
                    {
                        contractNumber
                    }
                },
                PushData = new Dictionary<string, object>
                {
                    {"TemplateCode", templateCode},
                    {"PackageId", packageId},
                    {"AppId", contractId},
                    {"ContractNumber", contractNumber},
                    {"ContractId", contractId}
                },
                SapNumber = sapNumber
            };
            
            
            using var queuePublisher = _serviceProvider.GetRequiredService<IQueuePublisher>();
            queuePublisher.Publish(message);
            await Task.CompletedTask;
        }
    }
}