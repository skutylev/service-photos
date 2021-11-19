using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Service.PhotoPackages.Dal
{
    public class PhotoPackagesDbContextFactory : IDesignTimeDbContextFactory<PhotoPackagesDbContext>
    {
        public PhotoPackagesDbContext CreateDbContext(string[] args)
        {
            var dalAssembly = typeof(DalModule).Assembly.FullName;
            var connectionString = Environment.GetEnvironmentVariable("ServicePhotoPackagesDal");
            var builder = new DbContextOptionsBuilder<PhotoPackagesDbContext>();

            builder.UseOracle(connectionString, optionsBuilder => optionsBuilder
                .MigrationsAssembly(dalAssembly)
                .MigrationsHistoryTable($"{Constants.ServicePrefix}_MIGRATIONS_HISTORY")
            );

            return new PhotoPackagesDbContext(builder.Options);
        }
    }
}