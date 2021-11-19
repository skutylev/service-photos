using MediatR;

namespace Service.PhotoPackages.ServiceLayer.MediatR.Commands.DeletePhotoContent
{
    public class DeletePhotoContentMCommand : IRequest
    {
        public string PhotoPackageId { get; set; }
        public string PhotoId { get; set; }
        public string ContactId { get; set; }
        public long AuthorSapNumber { get; set; }
    }
}