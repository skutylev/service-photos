using MediatR;

namespace Service.PhotoPackages.ServiceLayer.MediatR.Commands.ProcessPhotoPackageAccepting
{
    public class ProcessPhotoPackageAcceptingMCommand : IRequest
    {
        public string TypeCode { get; set; }
        public string ContractId { get; set; }
        public string PhotoPackageId { get; set; }
        public string PhotoPackageProcessTypeCode { get; set; }
        public long CourierSapNumber { get; set; }
        public long AuthorSapNumber { get; set; }
        public bool ProcessingCompleted { get; set; }
    }
}