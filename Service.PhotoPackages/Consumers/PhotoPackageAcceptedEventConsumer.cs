using System.Threading.Tasks;
using MassTransit;
using MediatR;
using Service.PhotoPackages.Events;
using Service.PhotoPackages.ServiceLayer.MediatR.Commands.ProcessPhotoPackageAccepting;

namespace Service.PhotoPackages.Consumers
{
    public class PhotoPackageAcceptedEventConsumer : IConsumer<PhotoPackageAcceptedEvent>
    {
        private readonly IMediator _mediator;

        public PhotoPackageAcceptedEventConsumer(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task Consume(ConsumeContext<PhotoPackageAcceptedEvent> context)
        {
            await _mediator.Send(new ProcessPhotoPackageAcceptingMCommand
            {
                TypeCode = context.Message.TypeCode,
                ContractId = context.Message.ContractId,
                PhotoPackageId = context.Message.Id,
                PhotoPackageProcessTypeCode = context.Message.ProcessTypeCode,
                CourierSapNumber = context.Message.CourierSapNumber,
                AuthorSapNumber = context.Message.AuthorSapNumber,
                ProcessingCompleted = context.Message.ProcessingCompleted
            }, context.CancellationToken);
        }
    }
}