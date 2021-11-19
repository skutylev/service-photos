namespace Service.PhotoPackages.Dal.Models.Base
{
    internal interface IBaseModelWithLongPrimaryKey 
    {
        public long Id { get; set; }
    }
}
