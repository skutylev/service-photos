namespace Service.PhotoPackages.ServiceLayer.Integrations.DMNs.PhotoTypesDmn
{
    public class PhotoTypesDmnRequest : IDmnRequest
    {
        public string ProductTypeCode { get; set; }
        public string ProductName { get; set; }
        public string SigningStatus { get; set; }
        public string PackagePhotoTypeCode { get; set; }
        public bool IsUnderage { get; set; }
    }
}