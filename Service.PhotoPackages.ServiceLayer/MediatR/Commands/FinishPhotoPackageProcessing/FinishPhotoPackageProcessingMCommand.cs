using MediatR;

namespace Service.PhotoPackages.ServiceLayer.MediatR.Commands.FinishPhotoPackageProcessing
{
    public class FinishPhotoPackageProcessingMCommand : IRequest
    {
        public string ContractId { get; set; }
        public string Id { get; set; }
        public long CourierSapNumber { get; set; }
        public long AuthorSapNumber { get; set; }
        public string ProcessTypeCode { get; set; }
        public string TypeCode { get; set; }
    }
}