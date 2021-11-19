using System;
using System.Collections.Generic;
using Service.PhotoPackages.Dal.Models.Base;

namespace Service.PhotoPackages.Dal.Models
{
    public class PhotoPackage : IBaseModelWithGuidPrimaryKey, IBaseModelWithStartDate, IBaseModelWithUpdateDate
    {
        public string Id { get; set; }
        public string BucketId { get; set; }
        public string TypeCode { get; set; }
        public string ContractId { get; set; }
        public string StatusCode { get; set; }
        public string ProcessCode { get; set; }
        public long? AuthorSapNumber { get; set; }
        public long? VerifierSapNumber { get; set; }
        public string ScanCheckerRequestId { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public string ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
        public DateTime ClientBirthday { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public virtual ICollection<Photo> Photos { get; set; }
        public virtual ICollection<PhotoPackageHistory> History { get; set; }
    }
}