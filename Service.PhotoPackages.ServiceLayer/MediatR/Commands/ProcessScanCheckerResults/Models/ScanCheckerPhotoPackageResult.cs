using System.Collections.Generic;

namespace Service.PhotoPackages.ServiceLayer.MediatR.Commands.ProcessScanCheckerResults.Models
{
    public class ScanCheckerPhotoPackageResult
    {
        public string PhotoPackageId { get; set; }
        public string ScanCheckerRequestId { get; set; }
        public string PhotoPackageStatusCode { get; set; }
        public string PhotoPackageErrorCode { get; set; }
        public string PhotoPackageErrorMessage { get; set; }
        public IEnumerable<ScanCheckerPhotoResult> PhotosResults { get; set; }
    }
}