using System.Collections.Generic;
using MediatR;

namespace Service.PhotoPackages.ServiceLayer.MediatR.Commands.UpdatePhotoPackageStatus
{
    public class UpdatePhotoPackageStatusMCommand : IRequest<string>
    {
        public string Id { get; set; }
        public string ContractId { get; set; }
        public string ProductTypeCode { get; set; }
        public string SigningStatusCode { get; set; }
        public string CallerType { get; set; }
        public string StatusCode { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public long AuthorSapNumber { get; set; }
        public string ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
        public List<(string photoId, string errorCode, string errorMessage, string statusCode)> PhotoErrors { get; set; }
    }
}