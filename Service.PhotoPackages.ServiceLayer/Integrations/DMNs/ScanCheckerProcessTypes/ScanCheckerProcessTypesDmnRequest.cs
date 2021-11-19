namespace Service.PhotoPackages.ServiceLayer.Integrations.DMNs.ScanCheckerProcessTypes
{
    public class ScanCheckerProcessTypesDmnRequest : IDmnRequest
    {
        public string ProductName { get; set; }
        public string PhotoPackageTypeCode { get; set; }
    }
}