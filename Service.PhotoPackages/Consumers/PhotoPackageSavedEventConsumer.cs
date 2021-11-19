using System.Threading.Tasks;
using MassTransit;
using MediatR;
using Service.PhotoPackages.Events;
using Service.PhotoPackages.ServiceLayer.MediatR.Commands.ProcessPhotoPackageSaving;

namespace Service.PhotoPackages.Consumers
{
    public class PhotoPackageSavedEventConsumer : IConsumer<PhotoPackageSavedEvent>
    {
        private readonly IMediator _mediator;

        public PhotoPackageSavedEventConsumer(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task Consume(ConsumeContext<PhotoPackageSavedEvent> context)
        {
            await _mediator.Send(new ProcessPhotoPackageSavedEventMCommand
            {
                ContractId = context.Message.ContractId,
                PhotoPackageId = context.Message.Id
            }, context.CancellationToken);
        }
    }
}