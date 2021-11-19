using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Service.PhotoPackages.Dal.Models;

namespace Service.PhotoPackages.Dal.Configurations
{
    public class PhotoConfiguration: IEntityTypeConfiguration<Photo>
    {
        private readonly string _tableName;
        
        public PhotoConfiguration(string tableName)
        {
            _tableName = $"{Constants.ServicePrefix}_{tableName.ToUpper()}";
        }
        
        public void Configure(EntityTypeBuilder<Photo> builder)
        {
            builder.ToTable(_tableName);
            
            builder.HasKey(p => p.Id)
                .HasName($"PK_{_tableName}");
            
            builder.Property(p => p.Id)
                .HasColumnName("ID")
                .HasColumnType(Constants.StringDbType)
                .HasMaxLength(36)
                .ValueGeneratedOnAdd()
                .HasValueGenerator<GuidIdGenerator>()
                .IsRequired();
            
            builder.Property(p => p.PhotoPackageId)
                .HasColumnName("PHOTO_PACKAGE_ID")
                .HasColumnType(Constants.StringDbType)
                .HasMaxLength(36)
                .IsRequired();
            
            builder.Property(p => p.ContentId)
                .HasColumnName("CONTENT_ID")
                .HasColumnType(Constants.StringDbType)
                .HasMaxLength(36);
            
            builder.Property(p => p.ThumbnailContentId)
                .HasColumnName("THUMBNAIL_CONTENT_ID")
                .HasColumnType(Constants.StringDbType)
                .HasMaxLength(36);
            
            builder.Property(p => p.TypeCode)
                .HasColumnName("TYPE_CODE")
                .HasColumnType(Constants.StringDbType)
                .HasMaxLength(50)
                .IsRequired();
            
            builder.Property(p => p.StatusCode)
                .HasColumnName("STATUS_CODE")
                .HasColumnType(Constants.StringDbType)
                .HasMaxLength(50)
                .IsRequired();
            
            builder.Property(p => p.Required)                
                .HasColumnName("REQUIRED")
                .HasColumnType(Constants.BoolDbType);
            
            builder.Property(p => p.CanBeRetaken)                
                .HasColumnName("CAN_BE_RETAKEN")
                .HasColumnType(Constants.BoolDbType);
            
            builder.Property(p => p.AttachSystemCode)
                .HasColumnName("ATTACH_SYSTEM_CODE")
                .HasColumnType(Constants.StringDbType)
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(p => p.ErrorCode)
                .HasColumnName("ERROR_CODE")
                .HasColumnType(Constants.StringDbType)
                .HasMaxLength(100);
            
            builder.Property(p => p.ErrorMessage)
                .HasColumnName("ERROR_MESSAGE")
                .HasColumnType(Constants.StringDbType)
                .HasMaxLength(1000);
            
            builder.Property(p => p.ScanServerDocumentId)
                .HasColumnName("SCAN_SERVER_DOCUMENT_ID")
                .HasColumnType(Constants.StringDbType)
                .HasMaxLength(36);
            
            builder.Property(p => p.ScanServerPageId)
                .HasColumnName("SCAN_SERVER_PAGE_ID")
                .HasColumnType(Constants.StringDbType)
                .HasMaxLength(36);
            
            builder.Property(p => p.ScanServerPageNumber)
                .HasColumnName("SCAN_SERVER_PAGE_NUMBER")
                .HasColumnType(Constants.StringDbType)
                .HasMaxLength(36);
            
            builder.Property(p => p.TranzWareScanId)
                .HasColumnName("TRANZ_WARE_SCAN_ID")
                .HasColumnType(Constants.StringDbType)
                .HasMaxLength(36);

            builder.Property(p => p.StartDate)
                .HasColumnName("START_DATE")
                .HasColumnType(Constants.DateDbType)
                .IsRequired();
            
            builder.Property(p => p.UpdateDate)
                .HasColumnName("UPDATE_DATE")
                .HasColumnType(Constants.DateDbType);

            builder.HasOne(s => s.PhotoPackage)
                .WithMany(p => p.Photos)
                .HasForeignKey(p => p.PhotoPackageId)
                .HasPrincipalKey(p => p.Id)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(s => s.History)
                .WithOne(p => p.Photo)
                .HasForeignKey(p => p.PhotoId)
                .HasPrincipalKey(p => p.Id)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}