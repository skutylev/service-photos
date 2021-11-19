namespace Service.PhotoPackages.ServiceLayer.MediatR.Commands.ProcessPhotoPackageSaving.Strategy
{
    public class ProcessResult
    {
        public string PhotoPackageStatusCode { get; set; }
        public string ScanCheckerRequestId { get; set; }
    }
}