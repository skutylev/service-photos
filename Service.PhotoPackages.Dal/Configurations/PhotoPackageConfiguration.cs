using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Service.PhotoPackages.Dal.Models;

namespace Service.PhotoPackages.Dal.Configurations
{
    public class PhotoPackageConfiguration : IEntityTypeConfiguration<PhotoPackage>
    {
        private readonly string _tableName;
        
        public PhotoPackageConfiguration(string tableName)
        {
            _tableName = $"{Constants.ServicePrefix}_{tableName.ToUpper()}";
        }
        
        public void Configure(EntityTypeBuilder<PhotoPackage> builder)
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
            
            builder.Property(p => p.BucketId)
                .HasColumnName("BUCKET_ID")
                .HasColumnType(Constants.StringDbType)
                .HasMaxLength(36)
                .IsRequired();
            
            builder.Property(p => p.TypeCode)
                .HasColumnName("TYPE_CODE")
                .HasColumnType(Constants.StringDbType)
                .HasMaxLength(50)
                .IsRequired();
            
            builder.Property(p => p.ContractId)
                .HasColumnName("CONTRACT_ID")
                .HasColumnType(Constants.StringDbType)
                .HasMaxLength(36)
                .IsRequired();
            
            builder.Property(p => p.StatusCode)
                .HasColumnName("STATUS_CODE")
                .HasColumnType(Constants.StringDbType)
                .HasMaxLength(50)
                .IsRequired();
            
            builder.Property(p => p.ProcessCode)
                .HasColumnName("PROCESS_CODE")
                .HasColumnType(Constants.StringDbType)
                .HasMaxLength(100);
            
            builder.Property(p => p.AuthorSapNumber)
                .HasColumnName("AUTHOR_SAP_NUMBER")
                .HasColumnType(Constants.LongDbType);
            
            builder.Property(p => p.VerifierSapNumber)
                .HasColumnName("VERIFIER_SAP_NUMBER")
                .HasColumnType(Constants.LongDbType);
            
            builder.Property(p => p.ScanCheckerRequestId)
                .HasColumnName("SCAN_CHECKER_REQUEST_ID")
                .HasColumnType(Constants.StringDbType)
                .HasMaxLength(36);

            builder.Property(p => p.Latitude)
                .HasColumnName("LATITUDE")
                .HasColumnType(Constants.DoubleDbType);
            
            builder.Property(p => p.Longitude)
                .HasColumnName("LONGITUDE")
                .HasColumnType(Constants.DoubleDbType);
            
            builder.Property(p => p.ErrorCode)
                .HasColumnName("ERROR_CODE")
                .HasColumnType(Constants.StringDbType)
                .HasMaxLength(100);
            
            builder.Property(p => p.ErrorMessage)
                .HasColumnName("ERROR_MESSAGE")
                .HasColumnType(Constants.StringDbType)
                .HasMaxLength(1000);
            
            builder.Property(p => p.ClientBirthday)
                .HasColumnName("CLIENT_BIRTHDAY")
                .HasColumnType(Constants.DateDbType)
                .IsRequired();
            
            builder.Property(p => p.StartDate)
                .HasColumnName("START_DATE")
                .HasColumnType(Constants.DateDbType)
                .IsRequired();
            
            builder.Property(p => p.UpdateDate)
                .HasColumnName("UPDATE_DATE")
                .HasColumnType(Constants.DateDbType);
            
            builder.HasMany(s => s.History)
                .WithOne(p => p.PhotoPackage)
                .HasForeignKey(p => p.PhotoPackageId)
                .HasPrincipalKey(p => p.Id)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}


