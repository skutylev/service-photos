using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Service.PhotoPackages.ServiceLayer.Constants;
using Service.PhotoPackages.ServiceLayer.Integrations.Contracts;

namespace Service.PhotoPackages.ServiceLayer.MediatR.Commands.SendPushes
{
    public class SendPushAcceptedMCommandHandler : AsyncRequestHandler<SendPushAcceptedMCommand>
    {
        private readonly IMessageService _messageService;

        public SendPushAcceptedMCommandHandler(IMessageService messageService)
        {
            _messageService = messageService;
        }

        protected override async Task Handle(SendPushAcceptedMCommand request, CancellationToken cancellationToken)
        {
            var parameters = new Dictionary<string, object>
            {
                {"ContractId", request.ContractId},
                {"ContractNumber", request.ContractId}
            };
            await _messageService.SendPush(PushTemplatesCodes.MbrPhotoApproved, parameters, request.ReceiverSapNumber);
        }
    }
}