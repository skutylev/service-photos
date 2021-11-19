using System.Threading.Tasks;
using MassTransit;
using MediatR;
using Service.PhotoPackages.Events;
using Service.PhotoPackages.ServiceLayer.MediatR.Commands.ClearPhotoPackage;

namespace Service.PhotoPackages.Consumers
{
    public class ClearPhotoPackageConsumer : IConsumer<ClearPhotoPackageCommand>
    {
        private readonly IMediator _mediator;

        public ClearPhotoPackageConsumer(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task Consume(ConsumeContext<ClearPhotoPackageCommand> context)
        {
            await _mediator.Send(new ClearPhotoPackageMCommand
            {
                ContractId = context.Message.ContractId,
                SapNumber = context.Message.SapNumber
            }, context.CancellationToken);
        }
    }
}