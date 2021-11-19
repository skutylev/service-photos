using System.Collections.Generic;
using MediatR;

namespace Service.PhotoPackages.ServiceLayer.MediatR.Commands.UpdateScanServerDataMCommand
{
    public class UpdateScanServerDataMCommand : IRequest
    {
        public class PhotoScanServerInfo
        {
            public string PhotoId { get; set; }
            public string ScanServerDocumentId { get; set; }
            public string ScanServerPageId { get; set; }
            public string ScanServerPageNumber { get; set; }
            public string TwScanDocumentId { get; set; }
         }
        
        public string PhotoPackageId { get; set; }
        public IEnumerable<PhotoScanServerInfo> PhotoInfos { get; set; }
        
    }
}