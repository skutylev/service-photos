using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Service.Photos.Dal.Configurations
{
    public class PhotoPackageConfiguration : IEntityTypeConfiguration<PhotoPackage>
    {
        public void Configure(EntityTypeBuilder<PhotoPackage> builder)
        {
            builder.HasMany(p => p.ApplicationPhotos)
                .WithOne()
                .HasForeignKey(p => p.PhotoPackageId);

            builder.Property(p => p.Type).HasDefaultValue(PackageType.Contract);

            builder.HasIndex(p => new
            {
                p.ApplicationId, p.Type
            }).IsUnique();
        }
    }
}