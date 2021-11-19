using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Service.Photos.Dal.Configurations
{
    public class PhotoPackageHistoryConfiguration : IEntityTypeConfiguration<PhotoPackageHistory>
    {
        public void Configure(EntityTypeBuilder<PhotoPackageHistory> builder)
        {
            builder.HasMany(h => h.ApplicationPhotosHistory)
                .WithOne()
                .HasForeignKey(h => h.PackageHistoryId);
        }
    }
}