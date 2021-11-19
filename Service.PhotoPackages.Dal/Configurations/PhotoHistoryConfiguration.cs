using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Service.PhotoPackages.Dal.Models;

namespace Service.PhotoPackages.Dal.Configurations
{
    public class PhotoHistoryConfiguration : IEntityTypeConfiguration<PhotoHistory>
    {
        private readonly string _tableName;

        public PhotoHistoryConfiguration(string tableName)
        {
            _tableName = $"{Constants.ServicePrefix}_{tableName.ToUpper()}";
        }

        public void Configure(EntityTypeBuilder<PhotoHistory> builder)
        {
            builder.ToTable(_tableName);

            builder.HasKey(p => p.Id)
                .HasName($"PK_{_tableName}");
                        
            builder.Property(p => p.Id)
                .HasColumnName("ID")
                .HasColumnType(Constants.LongDbType)
                .ValueGeneratedOnAdd()
                .IsRequired();

            builder.Property(p => p.PhotoId)
                .HasColumnName("PHOTO_ID")
                .HasColumnType(Constants.StringDbType)
                .HasMaxLength(36)
                .IsRequired();
            
            builder.Property(p => p.StatusCode)
                .HasColumnName("STATUS_CODE")
                .HasColumnType(Constants.StringDbType)
                .HasMaxLength(50);
            
            builder.Property(p => p.ContentId)
                .HasColumnName("CONTENT_ID")
                .HasColumnType(Constants.StringDbType)
                .HasMaxLength(36);
            
            builder.Property(p => p.ThumbnailContentId)
                .HasColumnName("THUMBNAIL_CONTENT_ID")
                .HasColumnType(Constants.StringDbType)
                .HasMaxLength(36);
            
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
            
            builder.HasIndex(p => p.PhotoId)
                .HasDatabaseName($"IX_{_tableName}_PHOTO_ID");

        }
    }
}