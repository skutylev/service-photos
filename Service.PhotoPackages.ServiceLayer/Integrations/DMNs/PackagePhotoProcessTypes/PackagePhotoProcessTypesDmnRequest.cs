namespace Service.PhotoPackages.ServiceLayer.Integrations.DMNs.PackagePhotoProcessTypes
{
    public class PackagePhotoProcessTypesDmnRequest : IDmnRequest
    {
        public string ProductTypeCode { get; set; }
        public string SigningStatus { get; set; }
        public bool CourierHasOnlineActivationRight { get; set; }
    }
}