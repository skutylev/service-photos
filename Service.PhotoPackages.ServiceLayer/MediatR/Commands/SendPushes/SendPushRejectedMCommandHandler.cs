using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Service.PhotoPackages.ServiceLayer.Constants;
using Service.PhotoPackages.ServiceLayer.Integrations.Contracts;

namespace Service.PhotoPackages.ServiceLayer.MediatR.Commands.SendPushes
{
    public class SendPushRejectedMCommandHandler : AsyncRequestHandler<SendPushRejectedMCommand>
    {
        private readonly IMessageService _messageService;

        public SendPushRejectedMCommandHandler(IMessageService messageService)
        {
            _messageService = messageService;
        }

        protected override async Task Handle(SendPushRejectedMCommand request, CancellationToken cancellationToken)
        {
            var parameters = new Dictionary<string, object>
            {
                {"ContractId", request.ContractId},
                {"ContractNumber", request.ContractId}
            };
            await _messageService.SendPush(PushTemplatesCodes.MbrPhotoError, parameters, request.ReceiverSapNumber);
        }
    }
}