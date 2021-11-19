using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MassTransit;
using MediatR;
using Service.Contracts.Client.Contracts.Contracts;
using Service.PhotoPackages.Dal;
using Service.PhotoPackages.Events;
using Service.PhotoPackages.ServiceLayer.Constants;
using Service.PhotoPackages.ServiceLayer.Integrations.Contracts;

namespace Service.PhotoPackages.ServiceLayer.MediatR.Commands.SendImagesToScanServer
{
    public class SendPhotosToScanServerMCommandHandler : AsyncRequestHandler<SendPhotosToScanServerMCommand>
    {
        private readonly ISettingsService _settingsService;
        private readonly IContractService _contractService;
        private readonly IBus _bus;

        public SendPhotosToScanServerMCommandHandler(IBus bus, ISettingsService settingsService,
            IContractService contractService)
        {
            _bus = bus;
            _settingsService = settingsService;
            _contractService = contractService;
        }

        protected override async Task Handle(SendPhotosToScanServerMCommand request,
            CancellationToken cancellationToken)
        {
            var contractInfo = await _contractService.GetContractInfo(request.ContractId, cancellationToken);
            
            var sendPhotosToScanServerImageRequests = request.PhotosInfo.Select(p => new
            {
                IntegrationProperties = GetPhotoIntegrationProperties(p.typeCode),
                p.id,
                p.contentId
            }).Select(p => new SendPhotosToScanServerImageRequest
            {
                PhotoId = p.id,
                ContentId = p.contentId,
                OtherSign = p.IntegrationProperties.OtherSign,
                TwScanTypeId = p.IntegrationProperties.TwScanTypeId,
                ScanServerDocIdent = p.IntegrationProperties.ScanServerDocIdent
            });
            
            await _bus.Publish(new SendPhotosToScanServerCommand
            {
                PhotoPackageId = request.Id,
                BucketId = request.BucketId,
                ContractNumber = request.TypeCode.Equals(PhotoPackageTypes.Contract) ? contractInfo.ContractNumber : null,
                ApplicationNumber = GetApplicationNumberForTwService(request.TypeCode, contractInfo),
                ScanServerObjectType = GetScanServerObjectType(request.TypeCode),
                NeedSendToTw = request.NeedSendToTw,
                TwScanType = GetTwScanType(request.TypeCode),
                Images = sendPhotosToScanServerImageRequests
            }, cancellationToken);
        }

        private string GetApplicationNumberForTwService(string photoPackageType, ContractItemDto contractInfo) =>
            photoPackageType switch
            {
                PhotoPackageTypes.ApplicationLk => contractInfo.AdditionalServices.GetValueOrDefault("LK"),
                PhotoPackageTypes.ApplicationVzr => contractInfo.AdditionalServices.GetValueOrDefault("VZR"),
                _ => null
            };

        private string GetScanServerObjectType(string photoPackageType) =>
            photoPackageType switch
            {
                PhotoPackageTypes.ApplicationLk => "APPLICATION",
                PhotoPackageTypes.ApplicationVzr => "APPLICATION",
                PhotoPackageTypes.Contract => "CONTRACT",
                _ => null
            };

        private string GetTwScanType(string photoPackageType) =>
            photoPackageType switch
            {
                PhotoPackageTypes.ApplicationLk => "APPNO",
                PhotoPackageTypes.ApplicationVzr => "APPNO",
                PhotoPackageTypes.Contract => "CONTRACTNO",
                _ => null
            };

        private PhotoIntegrationProperties GetPhotoIntegrationProperties(string photoTypeCode)
        {
            var integrationProperties = new PhotoIntegrationProperties();

            var properties = _settingsService
                .GetPhotoTypes()
                .FirstOrDefault(t => t.Code == photoTypeCode)?
                .Properties.ToList();

            integrationProperties.ScanServerDocIdent =
                properties?.FirstOrDefault(p => p.Name.Contains(PhotoTypeProperties.ScanServerDocIdent,
                    StringComparison.InvariantCultureIgnoreCase))?.Value;

            integrationProperties.TwScanTypeId =
                properties?.FirstOrDefault(p => p.Name.Contains(PhotoTypeProperties.TwScanTypeId,
                    StringComparison.InvariantCultureIgnoreCase))?.Value;
            
            integrationProperties.OtherSign =
                properties?.FirstOrDefault(p => p.Name.Contains(PhotoTypeProperties.OtherSign,
                    StringComparison.InvariantCultureIgnoreCase))?.Value;

            return integrationProperties;
        }
    }
}