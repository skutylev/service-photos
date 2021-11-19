using System;
using System.Collections.Generic;
using MediatR;

namespace Service.PhotoPackages.ServiceLayer.MediatR.Commands.CreatePhotoPackage
{
    public class CreatePhotoPackageMCommand : IRequest
    {
        public string ContractId { get; set; }
        public string BucketId { get; set; }
        public string ProductTypeCode { get; set; }
        public string ProductName { get; set; }
        public string SigningStatus { get; set; }
        public DateTime ClientBirthday { get; set; }
        public IEnumerable<string> AdditionalServicesCodes { get; set; }
    }
}