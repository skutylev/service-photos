using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Service.Photos.Dal.Configurations
{
    public class ApplicationPhotoConfiguration : IEntityTypeConfiguration<ApplicationPhoto>
    {
        public void Configure(EntityTypeBuilder<ApplicationPhoto> builder)
        {
            builder.HasOne(p => p.Attachment)
                .WithOne()
                .HasForeignKey<ApplicationPhoto>(p => p.AttachmentId);
        }
    }
}