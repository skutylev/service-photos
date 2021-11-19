using MediatR;

namespace Service.PhotoPackages.ServiceLayer.MediatR.Commands.SendPushes
{
    public class SendPushAcceptedMCommand : IRequest
    {
        public string ContractId { get; set; }
        public long ReceiverSapNumber { get; set; }
    }
}