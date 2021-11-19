using MediatR;

namespace Service.PhotoPackages.ServiceLayer.MediatR.Commands.SendPushes
{
    public class SendPushRejectedMCommand : IRequest
    {
        public string ContractId { get; set; }
        public long ReceiverSapNumber { get; set; }
    }
}