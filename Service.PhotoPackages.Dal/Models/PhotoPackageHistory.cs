using System;
using Service.PhotoPackages.Dal.Models.Base;

namespace Service.PhotoPackages.Dal.Models
{
    public class PhotoPackageHistory : IBaseModelWithLongPrimaryKey, IBaseModelWithStartDate
    {
        public long Id { get; set; }
        public string PhotoPackageId { get; set; }
        public string StatusCode { get; set; }
        public long? AuthorSapNumber { get; set; }
        public long? VerifierSapNumber { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public string ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
        public string ScanCheckerRequestId { get; set; }
        public DateTime StartDate { get; set; }
        public virtual PhotoPackage PhotoPackage { get; set; }
    }
}