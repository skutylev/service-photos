namespace Service.PhotoPackages.ServiceLayer.MediatR.Commands.ProcessPhotoPackageSaving.Strategy
{
    public interface IPackagePhotoProcessStrategyManager
    {
        IPackagePhotoProcessStrategy Get(string processCode);
    }
}