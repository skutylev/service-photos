namespace Service.PhotoPackages.ServiceLayer.MediatR.Commands.ProcessScanCheckerResults.Models
{
    public class ScanCheckerPhotoResult
    {
        public string PhotoId { get; set; }
        public string PhotoTypeCode { get; set; }
        public string PhotoStatusCode { get; set; }
        public string PhotoErrorCode { get; set; }
        public string PhotoErrorMessage{ get; set; }
        public string ScanCheckerPhotoType{ get; set; }
        public string ScanServerDocumentId{ get; set; }
        public string ScanServerPageId{ get; set; }
        public string ScanServerPageNumber{ get; set; }
    }
}