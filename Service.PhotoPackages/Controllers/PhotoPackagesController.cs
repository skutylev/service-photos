using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.PhotoPackages.Client.Contracts;
using Service.PhotoPackages.ServiceLayer.Constants;
using Service.PhotoPackages.ServiceLayer.MediatR.Commands.CreatePhotoPackage;
using Service.PhotoPackages.ServiceLayer.MediatR.Commands.DeletePhotoContent;
using Service.PhotoPackages.ServiceLayer.MediatR.Commands.SendPhotoDocumentsOnEmail;
using Service.PhotoPackages.ServiceLayer.MediatR.Commands.UpdatePhotoPackageStatus;
using Service.PhotoPackages.ServiceLayer.MediatR.Commands.UploadPhotoContent;
using Service.PhotoPackages.ServiceLayer.MediatR.Requests.GetPhotoPackage;

namespace Service.PhotoPackages.Controllers
{
    [ApiController, ApiVersion("1"), Produces("application/json")]
    [Route("v1/photo-packages")]
    public class PhotoPackagesController : ControllerBase
    {
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [HttpPost]
        public async Task<IActionResult> CreatePhotoPackage(
            [FromBody] CreatePhotoPackageRequest request,
            [FromServices] IMediator mediator,
            CancellationToken cancellationToken)
        {
            await mediator.Send(new CreatePhotoPackageMCommand
            {
                ContractId = request.ContractId,
                BucketId = request.BucketId,
                ProductName = request.ProductName,
                ProductTypeCode = request.ProductTypeCode,
                SigningStatus = request.SigningStatus,
                AdditionalServicesCodes = request.AdditionalServicesCodes
            }, cancellationToken);
            return NoContent();
        }

        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<PhotoPackageDto>))]
        [HttpGet("{contractId}")]
        public async Task<IActionResult> GetPhotoPackages(
            [FromServices] IMediator mediator,
            [FromRoute] string contractId, CancellationToken cancellationToken,
            [FromQuery] bool includePhotoHistory = false,
            [FromQuery] bool includePhotoPackageHistory = false,
            [FromQuery] string callerType = CallerTypes.Mobile
        )
        {
            return Ok(await mediator.Send(new GetPhotoPackageMRequest
            {
                ContractId = contractId,
                IncludePhotoPackageHistory = includePhotoPackageHistory,
                IncludePhotoHistory = includePhotoHistory,
                CallerType = callerType
            }, cancellationToken));
        }

        [HttpPost("{contractId:long}/{photoPackageId:guid}")]
        public async Task<IActionResult> UploadPhotosContent(
            [FromRoute] long contractId,
            [FromRoute] Guid photoPackageId,
            [FromHeader] long sapNumber,
            [FromForm(Name = "image")] ICollection<IFormFile> images,
            [FromServices] IMediator mediator,
            CancellationToken cancellationToken)
        {
            if (images is null)
                throw new ArgumentNullException(nameof(images), "Файлы изображений не приложены");

            var result = new List<BasePhotoDto>();

            foreach (var image in images)
            {
                await using var stream = new MemoryStream();
                await image.CopyToAsync(stream, cancellationToken);

                result.Add(await mediator.Send(new UploadPhotoContentMCommand
                {
                    ContractId = contractId.ToString(),
                    PackagePhotoId = photoPackageId.ToString(),
                    PhotoId = image.FileName.Split('.').FirstOrDefault(),
                    ContentName = image.FileName,
                    ContentType = image.ContentType,
                    SapNumber = sapNumber,
                    ContentData = stream.ToArray()
                }, cancellationToken));
            }

            return Ok(result);
        }

        [HttpPost("{contractId:long}/{photoPackageId:guid}/{photoId:guid}")]
        public async Task<IActionResult> UploadPhotoContent(
            [FromRoute] long contractId,
            [FromRoute] Guid photoPackageId,
            [FromRoute] Guid photoId,
            [FromHeader] long sapNumber,
            [FromForm(Name = "image")] IFormFile image,
            [FromServices] IMediator mediator,
            CancellationToken cancellationToken)
        {
            if (image is null)
                throw new ArgumentNullException(nameof(image), "Файл изображения не приложен");
            await using var stream = new MemoryStream();
            await image.CopyToAsync(stream, cancellationToken);
            var bytes = stream.ToArray();

            return Ok(await mediator.Send(new UploadPhotoContentMCommand
            {
                ContractId = contractId.ToString(),
                PackagePhotoId = photoPackageId.ToString(),
                PhotoId = photoId.ToString(),
                ContentName = image.FileName,
                ContentType = image.ContentType,
                SapNumber = sapNumber,
                ContentData = bytes
            }, cancellationToken));
        }

        [HttpDelete("{contractId:long}/{photoPackageId:guid}/{photoId:guid}")]
        public async Task<IActionResult> DeletePhotoContent(
            [FromRoute] long contractId,
            [FromRoute] Guid photoPackageId,
            [FromRoute] Guid photoId,
            [FromHeader] long sapNumber,
            [FromServices] IMediator mediator,
            CancellationToken cancellationToken)
        {
            await mediator.Send(new DeletePhotoContentMCommand
            {
                ContactId = contractId.ToString(),
                PhotoPackageId = photoPackageId.ToString(),
                PhotoId = photoId.ToString(),
                AuthorSapNumber = sapNumber
            }, cancellationToken);
            return NoContent();
        }

        [HttpPut("{contractId:long}/{photoPackageId:guid}/status")]
        public async Task<IActionResult> UpdatePhotoPackageStatus(
            [FromRoute] long contractId,
            [FromRoute] Guid photoPackageId,
            [FromBody] PhotoPackageStatusUpdateRequest request,
            [FromServices] IMediator mediator,
            CancellationToken cancellationToken,
            [FromQuery] string callerType = CallerTypes.Mobile)
        {
            if (!new List<string> {CallerTypes.Mobile, CallerTypes.Web}.Contains(callerType,
                StringComparer.InvariantCultureIgnoreCase))
                throw new ArgumentOutOfRangeException(nameof(callerType));

            var result = await mediator.Send(new UpdatePhotoPackageStatusMCommand
            {
                Id = photoPackageId.ToString(),
                ContractId = contractId.ToString(),
                CallerType = callerType.ToUpper(),
                AuthorSapNumber = request.Author?.SapNumber ?? 0,
                StatusCode = request.StatusCode,
                ErrorCode = request.ErrorCode,
                ErrorMessage = request.ErrorMessage,
                Latitude = request.Latitude,
                Longitude = request.Longitude,
                ProductTypeCode = request.ProductTypeCode,
                SigningStatusCode = request.SigningStatusCode,
                PhotoErrors = request.Photos?.Select(e => (e.Id, e.ErrorCode, e.ErrorMessage, e.StatusCode)).ToList()
            }, cancellationToken);

            return !string.IsNullOrEmpty(result) ? Ok(new {NextActionMessage = result}) : NoContent();
        }

        [HttpPost("{contractId:long}/send-documents-photo")]
        public async Task<IActionResult> SendDocumentsPhoto([FromRoute] long contractId, [FromQuery] long sapNumber,
            [FromServices] IMediator mediator, CancellationToken cancellationToken)
        {
            if (sapNumber == 0)
                throw new ArgumentNullException(nameof(sapNumber),
                    "Для отправки фотографий документов необходимо указать табельный номер сотрудника");

            await mediator.Send(new SendPhotoDocumentsOnEmailMCommand
            {
                ContractId = contractId.ToString(),
                SapNumber = sapNumber
            }, cancellationToken);
            return NoContent();
        }
        
        [HttpPost("{contractId:long}/download-documents-photo")]
        public async Task<IActionResult> DownloadDocumentsPhoto([FromRoute] long contractId, [FromQuery] long sapNumber,
            [FromServices] IMediator mediator, CancellationToken cancellationToken)
        {
            if (sapNumber == 0)
                throw new ArgumentNullException(nameof(sapNumber),
                    "Для отправки фотографий документов необходимо указать табельный номер сотрудника");

            await mediator.Send(new SendPhotoDocumentsOnEmailMCommand
            {
                ContractId = contractId.ToString(),
                SapNumber = sapNumber
            }, cancellationToken);
            return NoContent();
        }
    }
}