using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Service.PhotoPackages.ServiceLayer.Infrastructure;

namespace Service.PhotoPackages.ServiceLayer.MediatR.Commands.StartPhotoPackageProcessing
{
    public class StartPhotoPackageProcessingMCommandHandler : AsyncRequestHandler<StartPhotoPackageProcessingMCommand>
    {
        private readonly RedisStorage _redisStorage;

        private const string ProcessPrefix = "processing";


        public StartPhotoPackageProcessingMCommandHandler(RedisStorage redisStorage)
        {
            _redisStorage = redisStorage;
        }

        protected override async Task Handle(StartPhotoPackageProcessingMCommand request,
            CancellationToken cancellationToken)
        {
            await _redisStorage.SetKey(ProcessPrefix,request.ContractId,  new TimeSpan(DateTime.Now.Date.AddDays(1).Date.Ticks-DateTime.Now.Ticks));
        }
    }
}