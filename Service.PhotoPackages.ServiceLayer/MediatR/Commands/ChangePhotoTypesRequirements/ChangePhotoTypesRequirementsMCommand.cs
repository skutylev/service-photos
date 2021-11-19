using System.Collections.Generic;
using MediatR;

namespace Service.PhotoPackages.ServiceLayer.MediatR.Commands.ChangePhotoTypesRequirements
{
    public class ChangePhotoTypesRequirementsMCommand : IRequest
    {
        public string ContractId { get; set; }
        public string PhotoPackageId { get; set; }
        public IEnumerable<(string photoId, bool required)> PhotoRequirements { get; set; }
    }
}