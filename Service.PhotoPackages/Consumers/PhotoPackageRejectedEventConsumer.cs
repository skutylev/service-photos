using System.Threading.Tasks;
using MassTransit;
using MediatR;
using Service.PhotoPackages.Events;
using Service.PhotoPackages.ServiceLayer.MediatR.Commands.ProcessPhotoPackageRejecting;

namespace Service.PhotoPackages.Consumers
{
    public class PhotoPackageRejectedEventConsumer : IConsumer<PhotoPackageRejectedEvent>
    {
        private readonly IMediator _mediator;

        public PhotoPackageRejectedEventConsumer(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task Consume(ConsumeContext<PhotoPackageRejectedEvent> context)
        {
            await _mediator.Send(new ProcessPhotoPackageRejectingMCommand
            {
                ContractId = context.Message.ContractId,
                PhotoPackageId = context.Message.Id,
                PhotoPackageProcessTypeCode = context.Message.ProcessTypeCode,
                CourierSapNumber = context.Message.CourierSapNumber,
                AuthorSapNumber = context.Message.AuthorSapNumber
            }, context.CancellationToken);
        }
    }
}