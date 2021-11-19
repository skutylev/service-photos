using System.Threading.Tasks;
using MassTransit;
using MediatR;
using Service.PhotoPackages.Events;
using Service.PhotoPackages.ServiceLayer.MediatR.Commands.CreatePhotoPackage;

namespace Service.PhotoPackages.Consumers
{
    public class CreatePhotoPackageCommandConsumer : IConsumer<CreatePhotoPackageCommand>
    {
        private readonly IMediator _mediator;

        public CreatePhotoPackageCommandConsumer(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task Consume(ConsumeContext<CreatePhotoPackageCommand> context)
        {
            await _mediator.Send(new CreatePhotoPackageMCommand
            {
                ContractId = context.Message.ContractId,
                BucketId = context.Message.BucketId,
                ProductName = context.Message.ProductName,
                ProductTypeCode = context.Message.ProductTypeCode,
                SigningStatus = context.Message.SigningStatus,
                ClientBirthday = context.Message.ClientBirthday,
                AdditionalServicesCodes = context.Message.AdditionalServicesCodes
            }, context.CancellationToken);
        }
    }
}