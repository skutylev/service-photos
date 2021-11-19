using System.Collections.Generic;
using MediatR;

namespace Service.PhotoPackages.ServiceLayer.MediatR.Commands.SendImagesToScanServer
{
    public class SendPhotosToScanServerMCommand : IRequest
    {
        public string Id { get; set; }
        public string ContractId { get; set; }
        public string BucketId { get; set; }
        public string TypeCode { get; set; }
        public bool NeedSendToTw { get; set; }
        public string ProcessTypeCode { get; set; }
        public IEnumerable<(string id, string typeCode, string contentId)> PhotosInfo { get; set; }

    }
}