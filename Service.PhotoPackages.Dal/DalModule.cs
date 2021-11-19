using Mbr.Bootstraper.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Service.PhotoPackages.Dal
{
    public class DalModule : ISettingsModule
    {
        public void Configure(IServiceCollection services, IConfiguration config)
        {
            var dalAssembly = typeof(DalModule).Assembly.GetName().Name;
            services.AddDbContext<PhotoPackagesDbContext>(opt =>
                opt.UseOracle(config.GetConnectionString($"{dalAssembly}"),
                    options =>
                    {
                        options.MigrationsAssembly(dalAssembly);
                        options.MigrationsHistoryTable($"{Constants.ServicePrefix}_MIGRATIONS_HISTORY");
                    }
                )
            );
        }
    }
}