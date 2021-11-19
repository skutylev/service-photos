using Mbr.Bootstraper.Contracts;
using Mbr.Dal;
using Mbr.Lib.RestClient.Refit;
using Mbr.QueueProvider.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Service.BlobStorage.Client;
using Service.Contracts.Client;
using Service.Integrations.Client;
using Service.Messages.Client;
using Service.PhotoPackages.ServiceLayer.Infrastructure;
using Service.PhotoPackages.ServiceLayer.Integrations;
using Service.PhotoPackages.ServiceLayer.Integrations.Contracts;
using Service.PhotoPackages.ServiceLayer.Integrations.DMNs;
using Service.PhotoPackages.ServiceLayer.MediatR.Commands.CreatePhotoPackage;
using Service.PhotoPackages.ServiceLayer.MediatR.Commands.ProcessPhotoPackageSaving.Strategy;
using Service.PhotoPackages.ServiceLayer.Migrator;
using Service.PhotoPackages.ServiceLayer.Utils;
using Service.Photos.Dal;
using Service.Users.Client;

namespace Service.PhotoPackages.ServiceLayer
{
    public class ServiceModule : ISettingsModule
    {
        public void Configure(IServiceCollection services, IConfiguration config)
        {
            services.AddRefitHttpClient<UserServiceClient>(config);
            services.AddRefitHttpClient<IntegrationsServiceClient>(config);
            services.AddRefitHttpClient<ServiceMessagesClient>(config);
            services.AddRefitHttpClient<ServiceBlobStorageClient>(config);
            services.AddRefitHttpClient<ServiceContractsClient>(config);
            
            services.AddMediatR(typeof(CreatePhotoPackageMCommandHandler).Assembly);
            services.AddScoped(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
            services.AddQueue(config);
            
            services.AddScoped<IPhotoDmnService, PhotoDmnService>();
            services.AddScoped<ISettingsService, SettingsService>();
            services.AddScoped<IUsersService, UsersService>();
            services.AddScoped<IBlobStorageService, BlobStorageService>();
            services.AddScoped<IPhotoConverterService, PhotoConverterService>();
            services.AddScoped<IMessageService, MessageService>();
            services.AddScoped<IContractService, ContractService>();
            services.AddScoped<IScanCheckerService, ScanCheckerService>();
            services.AddScoped<IMigratorService, MigratorService>();

            services.AddScoped<IPackagePhotoProcessStrategyManager, PackagePhotoProcessStrategyManager>();
            services.AddScoped<CreditOnlinePackagePhotoProcessStrategy>();
            services.AddScoped<CreditSignedPackagePhotoProcessStrategy>();
            services.AddScoped<DebitOnlinePackagePhotoProcessStrategy>();
            services.AddScoped<DebitOfflinePackagePhotoProcessStrategy>();
            services.AddScoped<DebitSignedPackagePhotoProcessStrategy>();
            services.AddScoped<RedisStorage>();
            
            services.AddDbContext<MbrDbContext>(opt =>
                opt.UseOracle(config.GetConnectionString("MbrConnectionString"),
                    options =>
                    {
                        options.MigrationsAssembly(typeof(MbrDbContext).Assembly.GetName().Name);
                    }
                )
            );
            
            services.AddDbContext<PhotosContext>(opt =>
                opt.UseOracle(config.GetConnectionString(typeof(PhotosContext).Assembly.GetName().Name),
                    options =>
                    {
                        options.MigrationsAssembly(typeof(PhotosContext).Assembly.GetName().Name);
                        options.MigrationsHistoryTable($"PHOTOS_MIGRATIONS_HISTORY");
                    }
                )
            );
        }
    }
}