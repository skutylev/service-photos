using System;
using Service.PhotoPackages.Dal.Models.Base;

namespace Service.PhotoPackages.Dal.Models
{
    public class PhotoHistory : IBaseModelWithLongPrimaryKey, IBaseModelWithStartDate
    {
        public long Id { get; set; }
        public string StatusCode { get; set; }
        public string PhotoId { get; set; }
        public string ContentId { get; set; }
        public string ThumbnailContentId { get; set; }
        public string ErrorCode{ get; set; }
        public string ErrorMessage { get; set; }
        public string ScanServerDocumentId { get; set; }
        public string ScanServerPageId { get; set; }
        public string ScanServerPageNumber { get; set; }
        public string TranzWareScanId { get; set; }
        public DateTime StartDate { get; set; }
        public virtual Photo Photo { get; set; }
    }
}