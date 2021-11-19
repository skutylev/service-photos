using System;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Service.PhotoPackages.Dal;

namespace Service.PhotoPackages.Filters
{
    public class MigrationApplyStartupFilter : IStartupFilter
    {
        
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public MigrationApplyStartupFilter(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }

        public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
        {
            using (var serviceScope = _serviceScopeFactory.CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetRequiredService<PhotoPackagesDbContext>();
                var logger = serviceScope.ServiceProvider.GetRequiredService<ILogger>();

                logger.Warning("PendingMigrations {@migrations}", context.Database.GetPendingMigrations().ToList());
                logger.Warning("AppliedMigrations {@migrations}", context.Database.GetAppliedMigrations().ToList());

                context.Database.Migrate();
                return next;
            }
        }
    }
}