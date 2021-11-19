using System.Threading;
using System.Threading.Tasks;
using MassTransit;
using MediatR;
using Service.PhotoPackages.Events;
using Service.PhotoPackages.ServiceLayer.Infrastructure;


namespace Service.PhotoPackages.ServiceLayer.MediatR.Commands.FinishPhotoPackageProcessing
{
    public class FinishPhotoPackageProcessingMCommandHandler : AsyncRequestHandler<FinishPhotoPackageProcessingMCommand>
    {
        private readonly RedisStorage _redisStorage;
        private const string ProcessPrefix = "processing";
        private readonly IBus _bus;

        public FinishPhotoPackageProcessingMCommandHandler(RedisStorage redisStorage, IBus bus)
        {
            _redisStorage = redisStorage;
            _bus = bus;
        }

        protected override async Task Handle(FinishPhotoPackageProcessingMCommand request, CancellationToken cancellationToken)
        {
            await _redisStorage.DeleteKey(ProcessPrefix, request.ContractId);
            await _bus.Publish(new PhotoPackageAcceptedEvent
            {
                Id = request.Id,
                ContractId = request.ContractId,
                CourierSapNumber = request.CourierSapNumber,
                AuthorSapNumber = request.AuthorSapNumber,
                ProcessTypeCode = request.ProcessTypeCode,
                TypeCode = request.TypeCode,
                ProcessingCompleted = true
            }, cancellationToken);
        }
    }
}