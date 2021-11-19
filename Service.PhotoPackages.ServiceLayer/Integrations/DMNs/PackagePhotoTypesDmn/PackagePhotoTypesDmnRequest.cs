namespace Service.PhotoPackages.ServiceLayer.Integrations.DMNs.PackagePhotoTypesDmn
{
    public class PackagePhotoTypesDmnRequest : IDmnRequest
    {
        public string ProductTypeCode { get; set; }
        public string ProductName { get; set; }
        public bool AdditionalServiceLk { get; set; }
        public bool AdditionalServiceVzr { get; set; }
    }
}