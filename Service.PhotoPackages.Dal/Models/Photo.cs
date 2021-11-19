using System;
using System.Collections.Generic;
using Service.PhotoPackages.Dal.Models.Base;

namespace Service.PhotoPackages.Dal.Models
{
    public class Photo : IBaseModelWithGuidPrimaryKey, IBaseModelWithStartDate, IBaseModelWithUpdateDate
    {
        public string Id { get; set; }
        public string PhotoPackageId { get; set; }
        public string ContentId { get; set; }
        public string ThumbnailContentId { get; set; }
        public string TypeCode { get; set; }
        public string StatusCode { get; set; }
        public bool Required { get; set; }
        public bool CanBeRetaken { get; set; }
        public string AttachSystemCode { get; set; }
        public string ScanServerDocumentId { get; set; }
        public string ScanServerPageId { get; set; }
        public string ScanServerPageNumber { get; set; }
        public string TranzWareScanId { get; set; }
        public string ErrorCode{ get; set; }
        public string ErrorMessage { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public virtual PhotoPackage PhotoPackage { get; set; }
        public virtual ICollection<PhotoHistory> History { get; set; }
    }
}