using System.Collections.Generic;
using MediatR;
using Service.PhotoPackages.Client.Contracts;

namespace Service.PhotoPackages.ServiceLayer.MediatR.Requests.GetPhotoPackage
{
    public class GetPhotoPackageMRequest : IRequest<IEnumerable<PhotoPackageDto>>
    {
        public string ContractId { get; set; }
        public bool IncludePhotoPackageHistory { get; set; }
        public bool IncludePhotoHistory { get; set; }
        public string CallerType { get; set; }
    }
}