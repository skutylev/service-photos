using MediatR;

namespace Service.PhotoPackages.ServiceLayer.MediatR.Commands.ProcessPhotoPackageRejecting
{
    public class ProcessPhotoPackageRejectingMCommand : IRequest
    {
        public string ContractId { get; set; }
        public string PhotoPackageId { get; set; }
        public string PhotoPackageProcessTypeCode { get; set; }
        public long CourierSapNumber { get; set; }
        public long AuthorSapNumber { get; set; }
    }
}