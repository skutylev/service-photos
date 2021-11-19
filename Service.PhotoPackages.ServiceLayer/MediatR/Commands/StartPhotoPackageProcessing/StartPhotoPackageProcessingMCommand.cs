using MediatR;

namespace Service.PhotoPackages.ServiceLayer.MediatR.Commands.StartPhotoPackageProcessing
{
    public class StartPhotoPackageProcessingMCommand : IRequest
    {
        public string ContractId { get; set; }
    }
}