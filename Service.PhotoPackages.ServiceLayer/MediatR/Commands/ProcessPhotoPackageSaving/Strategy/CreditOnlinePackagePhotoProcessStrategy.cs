using System;
using System.Threading;
using System.Threading.Tasks;
using Service.PhotoPackages.Dal;
using Service.PhotoPackages.Dal.Models;
using Service.PhotoPackages.ServiceLayer.Integrations.Contracts;

namespace Service.PhotoPackages.ServiceLayer.MediatR.Commands.ProcessPhotoPackageSaving.Strategy
{
    public class CreditOnlinePackagePhotoProcessStrategy : IPackagePhotoProcessStrategy
    {
        private readonly ISettingsService _settingsService;
        private readonly IMessageService _messageService;
        private readonly IContractService _contractService;

        public CreditOnlinePackagePhotoProcessStrategy(ISettingsService settingsService, IMessageService messageService, IContractService contractService)
        {
            _settingsService = settingsService;
            _messageService = messageService;
            _contractService = contractService;
        }

        public async Task<ProcessResult> Apply(PhotoPackage photoPackage, CancellationToken cancellationToken)
        {
            var contractInfo = await _contractService.GetContractInfo(photoPackage.ContractId, cancellationToken);
            var uri = new Uri($"{_settingsService.GetUviaBaseUrl()}/{photoPackage.Id}?contractId={photoPackage.ContractId}");
            var receivers = _settingsService.GetUviaEmails();
            var subject = $"Фото от Мобильного Банкира дог. № {contractInfo.ContractNumber}";
            var body = $"<a href=\"{uri}\">Проверить документы</a>";
            await _messageService.SendEmail(receivers, subject, body);
            
            return new ProcessResult
            {
                PhotoPackageStatusCode = PhotoPackageStatuses.Pending,
            };
        }
    }
}