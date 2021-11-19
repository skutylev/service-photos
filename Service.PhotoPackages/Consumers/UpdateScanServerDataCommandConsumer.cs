using System.Linq;
using System.Threading.Tasks;
using MassTransit;
using MediatR;
using Service.PhotoPackages.Events;
using Service.PhotoPackages.ServiceLayer.MediatR.Commands.UpdateScanServerDataMCommand;

namespace Service.PhotoPackages.Consumers
{
    public class UpdateScanServerDataCommandConsumer : IConsumer<UpdateScanServerDataCommand>
    {
        private readonly IMediator _mediator;

        public UpdateScanServerDataCommandConsumer(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task Consume(ConsumeContext<UpdateScanServerDataCommand> context)
        {
            await _mediator.Send(new UpdateScanServerDataMCommand
            {
                PhotoPackageId = context.Message.PhotoPackageId,
                PhotoInfos = context.Message.UpdateScanServerDataImages.Select(p =>
                    new UpdateScanServerDataMCommand.PhotoScanServerInfo
                    {
                        PhotoId = p.PhotoId,
                        ScanServerDocumentId = p.ScanServerDocumentId,
                        ScanServerPageId = p.ScanServerPageId,
                        ScanServerPageNumber = p.ScanServerPageNumber,
                        TwScanDocumentId = p.TwScanDocumentId
                    })
            }, context.CancellationToken);
        }
    }
}