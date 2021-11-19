using System;
using Microsoft.Extensions.DependencyInjection;
using Service.PhotoPackages.ServiceLayer.Constants;

namespace Service.PhotoPackages.ServiceLayer.MediatR.Commands.ProcessPhotoPackageSaving.Strategy
{
    public class PackagePhotoProcessStrategyManager : IPackagePhotoProcessStrategyManager
    {
        private readonly IServiceProvider _serviceProvider;

        public PackagePhotoProcessStrategyManager(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IPackagePhotoProcessStrategy Get(string processCode)
        {
            return processCode switch
            {
                PhotoPackageProcessTypes.CreditOnline => _serviceProvider
                    .GetRequiredService<CreditOnlinePackagePhotoProcessStrategy>(),
                PhotoPackageProcessTypes.CreditSigned => _serviceProvider
                    .GetRequiredService<CreditSignedPackagePhotoProcessStrategy>(),
                PhotoPackageProcessTypes.DebitOnline => _serviceProvider
                    .GetRequiredService<DebitOnlinePackagePhotoProcessStrategy>(),
                PhotoPackageProcessTypes.DebitOffline => _serviceProvider
                    .GetRequiredService<DebitOfflinePackagePhotoProcessStrategy>(),
                PhotoPackageProcessTypes.DebitSigned => _serviceProvider
                    .GetRequiredService<DebitSignedPackagePhotoProcessStrategy>(),
                _ => throw new ArgumentOutOfRangeException(nameof(processCode), $"Работа со стратегией {processCode} не предусмотрена")
            };
        }
    }
}