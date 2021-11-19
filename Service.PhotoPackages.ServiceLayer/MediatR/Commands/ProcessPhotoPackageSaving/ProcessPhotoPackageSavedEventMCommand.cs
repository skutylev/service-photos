using MediatR;

namespace Service.PhotoPackages.ServiceLayer.MediatR.Commands.ProcessPhotoPackageSaving
{
    public class ProcessPhotoPackageSavedEventMCommand : IRequest
    {
        public string PhotoPackageId { get; set; }
        public string ContractId { get; set; }
    }
}