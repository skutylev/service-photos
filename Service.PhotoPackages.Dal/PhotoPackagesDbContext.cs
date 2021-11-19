using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Service.PhotoPackages.Dal.Configurations;
using Service.PhotoPackages.Dal.Models;
using Service.PhotoPackages.Dal.Models.Base;

namespace Service.PhotoPackages.Dal
{
    public class PhotoPackagesDbContext : DbContext
    {
        public DbSet<PhotoPackage> PhotoPackages { get; set; }
        public DbSet<PhotoPackageHistory> PhotoPackagesHistory { get; set; }
        public DbSet<Photo> Photos { get; set; }
        public DbSet<PhotoHistory> PhotosHistory { get; set; }

        public PhotoPackagesDbContext(DbContextOptions<PhotoPackagesDbContext> options) : base(options)
        {
        }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new PhotoPackageConfiguration("PHOTO_PACKAGE"));
            modelBuilder.ApplyConfiguration(new PhotoPackageHistoryConfiguration("PHOTO_PACKAGE_HISTORY"));
            modelBuilder.ApplyConfiguration(new PhotoConfiguration("PHOTO"));
            modelBuilder.ApplyConfiguration(new PhotoHistoryConfiguration("PHOTO_HISTORY"));
            
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            foreach (var entry in ChangeTracker.Entries<IBaseModelWithStartDate>()
                .Where(x => x.State == EntityState.Added))
            {
                entry.Entity.StartDate = DateTime.UtcNow;
            }

            foreach (var entry in ChangeTracker.Entries<IBaseModelWithUpdateDate>()
                .Where(x => x.State == EntityState.Modified))
            {
                entry.Entity.UpdateDate = DateTime.UtcNow;
            }

            return base.SaveChangesAsync(cancellationToken);
        }
    }
}