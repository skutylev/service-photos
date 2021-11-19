namespace Service.PhotoPackages.Dal.Models.Base
{
    internal interface IBaseModelWithGuidPrimaryKey 
    {
        public string Id { get; set; }
    }
}