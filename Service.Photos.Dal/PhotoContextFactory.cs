using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Service.Photos.Dal
{
    public class PhotoContextFactory : IDesignTimeDbContextFactory<PhotosContext>
    {
        public PhotosContext CreateDbContext(string[] args)
        {
            var connectionString = Environment.GetEnvironmentVariable("MbrConnection");

            var builder = new DbContextOptionsBuilder<PhotosContext>();
            builder.UseOracle(connectionString,
                optionsBuilder => optionsBuilder
                    .MigrationsAssembly(typeof(PhotosContext).Assembly.FullName)
                    .MigrationsHistoryTable($"{DalConstants.Schema}_MIGRATIONS_HISTORY"));

            return new PhotosContext(builder.Options);
        }
    }
}