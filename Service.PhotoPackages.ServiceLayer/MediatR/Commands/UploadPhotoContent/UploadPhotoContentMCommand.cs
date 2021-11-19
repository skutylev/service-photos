using MediatR;
using Service.PhotoPackages.Client.Contracts;

namespace Service.PhotoPackages.ServiceLayer.MediatR.Commands.UploadPhotoContent
{
    public class UploadPhotoContentMCommand : IRequest<BasePhotoDto>
    {
        public string ContractId { get; set; }
        public string PackagePhotoId { get; set; }
        public string PhotoId { get; set; }
        public long SapNumber { get; set; }
        public string ContentName { get; set; }
        public string ContentType { get; set; }
        public byte[] ContentData { get; set; }
    }
}