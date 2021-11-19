using MediatR;

namespace Service.PhotoPackages.ServiceLayer.MediatR.Commands.ClearPhotoPackage
{
    public class ClearPhotoPackageMCommand : IRequest
    {
        public string ContractId { get; set; }
        public long SapNumber { get; set; }
    }
}