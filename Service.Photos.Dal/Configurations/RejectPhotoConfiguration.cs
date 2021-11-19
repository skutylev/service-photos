using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Service.Photos.Dal.Configurations
{
    public class RejectPhotoConfiguration : IEntityTypeConfiguration<ApplicationRejectPhoto>
    {
        public void Configure(EntityTypeBuilder<ApplicationRejectPhoto> builder)
        {
            builder.HasOne(p => p.Attachment)
                .WithOne()
                .HasForeignKey<ApplicationRejectPhoto>(p => p.AttachmentId);
        }
    }
}