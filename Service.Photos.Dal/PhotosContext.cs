using Microsoft.EntityFrameworkCore;

namespace Service.Photos.Dal
{
    public class PhotosContext : DbContext
    {
        public PhotosContext(DbContextOptions options) : base(options)
        {
            
        }

        public DbSet<PhotoPackage> PhotoPackages { get; set; }
        public DbSet<ApplicationPhoto> ApplicationPhotos { get; set; }
        public DbSet<PhotoPackageHistory> PhotoPackageHistory { get; set; }
        public DbSet<CurrentPhotoType> CurrentPhotoTypes { get; set; }
        public DbSet<CurrentPhotoRequired> CurrentPhotosRequired { get; set; }
        public DbSet<Attachment> Attachments { get; set; }
        public DbSet<ApplicationRejectPhoto> ApplicationRejectPhotos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(PhotosContext).Assembly);
        }
    }
}