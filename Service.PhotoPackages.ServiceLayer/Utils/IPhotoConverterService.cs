namespace Service.PhotoPackages.ServiceLayer.Utils
{
    public interface IPhotoConverterService
    {
        byte[] ConvertToThumbnail(byte[] photoBytes);
    }
}