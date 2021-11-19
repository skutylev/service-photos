using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MassTransit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Service.PhotoPackages.Dal;
using Service.PhotoPackages.Dal.Models;
using Service.PhotoPackages.Events;
using Service.PhotoPackages.ServiceLayer.Constants;
using Service.PhotoPackages.ServiceLayer.Integrations.Contracts;
using Service.PhotoPackages.ServiceLayer.Integrations.DMNs.PackagePhotoProcessTypes;
using Service.PhotoPackages.ServiceLayer.MediatR.Commands.StartPhotoPackageProcessing;

namespace Service.PhotoPackages.ServiceLayer.MediatR.Commands.UpdatePhotoPackageStatus
{
    public class UpdatePhotoPackageStatusMCommandHandler : IRequestHandler<UpdatePhotoPackageStatusMCommand, string>
    {
        private readonly PhotoPackagesDbContext _dbContext;
        private readonly IPhotoDmnService _photoDmnService;
        private readonly ISettingsService _settingsService;
        private readonly IUsersService _usersService;
        private readonly IBus _bus;
        private readonly IMediator _mediator;

        private enum RoleRights
        {
            Opdk = 1
        }

        public UpdatePhotoPackageStatusMCommandHandler(PhotoPackagesDbContext dbContext,
            IPhotoDmnService photoDmnService, IUsersService usersService, IBus bus, ISettingsService settingsService, IMediator mediator)
        {
            _dbContext = dbContext;
            _photoDmnService = photoDmnService;
            _usersService = usersService;
            _bus = bus;
            _settingsService = settingsService;
            _mediator = mediator;
        }

        public async Task<string> Handle(UpdatePhotoPackageStatusMCommand request,
            CancellationToken cancellationToken)
        {
            return request.CallerType.ToUpper() switch
            {
                CallerTypes.Mobile => await HandleSaved(request, cancellationToken),
                CallerTypes.Web => await HandleVerification(request, cancellationToken),
                CallerTypes.System => await HandleVerification(request, cancellationToken),
                _ => throw new NotImplementedException()
            };
        }
        

        private async Task<string> HandleSaved(UpdatePhotoPackageStatusMCommand request,
            CancellationToken cancellationToken)
        {
            if (!request.StatusCode.Equals(PhotoPackageStatuses.Saved, StringComparison.InvariantCultureIgnoreCase))
                throw new ArgumentOutOfRangeException($"{request.StatusCode}", "Установка данного статуса запрещена");

            var photoPackage = await GetPhotoPackage(request.ContractId, request.Id, cancellationToken);

            if (request.Latitude == 0.0 && request.Longitude == 0.0)
                throw new ArgumentOutOfRangeException(nameof(request.Latitude), "Отправка пакета фото без координат места фотографирования запрещена");
            
            if (!new List<string> {PhotoPackageStatuses.New, PhotoPackageStatuses.Rejected}.Contains(photoPackage
                .StatusCode))
                throw new ArgumentOutOfRangeException(nameof(photoPackage.StatusCode),
                    $"Отправка пакета фото {photoPackage.Id} в статусе {photoPackage.StatusCode} на проверку не предусмотрена");

            if (photoPackage.Photos.Where(p => p.Required).Any(p => p.ContentId == null || !string.IsNullOrEmpty(p.ErrorCode)))
                throw new ArgumentOutOfRangeException(nameof(photoPackage.Id),
                    "Не все обязательные фотографии были приложены");

            var author = (await _usersService.GetUsers(new[] {request.AuthorSapNumber}, cancellationToken))
                         .FirstOrDefault() ??
                         throw new ArgumentOutOfRangeException($"{photoPackage.Id}",
                             "Пользователь не найден в системе, отправка фото запрещена");

            var processCode = _photoDmnService.GetPackagePhotoProcessTypes(new PackagePhotoProcessTypesDmnRequest
            {
                CourierHasOnlineActivationRight = author.Rights.HasValue &&
                                                  ((RoleRights?) author.Rights).Value.HasFlag(RoleRights.Opdk),
                ProductTypeCode = request.ProductTypeCode,
                SigningStatus = request.SigningStatusCode
            }).ProcessTypeCode;

            var process = _settingsService.GetPhotoPackageProcessTypes().FirstOrDefault(t => t.Code == processCode) ??
                          throw new ArgumentNullException(nameof(processCode),
                              "Определенный тип процесса не найден в справочнике");

            var nextActionMessage = process.Properties.FirstOrDefault(p =>
                                        p.Name.Equals(PhotoTypeProperties.NextActionMessage,
                                            StringComparison.InvariantCultureIgnoreCase)) ??
                                    throw new ArgumentNullException(nameof(processCode),
                                        "Для определенного типа процесса не указано сообщение о дальнейших действиях в справочнике");

            
            photoPackage.ProcessCode = processCode;
            photoPackage.StatusCode = request.StatusCode;
            photoPackage.Latitude = request.Latitude;
            photoPackage.Longitude = request.Longitude;
            photoPackage.AuthorSapNumber = author.SapNumber;
            photoPackage.VerifierSapNumber = null;
            photoPackage.ErrorCode = null;
            photoPackage.ErrorMessage = null;

            await _dbContext.SaveChangesAsync(cancellationToken);

            await _bus.Publish(new PhotoPackageSavedEvent
            {
                Id = photoPackage.Id,
                ContractId = photoPackage.ContractId,
                CourierSapNumber = photoPackage.AuthorSapNumber ?? 0,
                ProcessTypeCode = photoPackage.ProcessCode,
                TypeCode = photoPackage.TypeCode
            }, cancellationToken);
            return nextActionMessage.Value;
        }


        private async Task<string> HandleVerification(UpdatePhotoPackageStatusMCommand request,
            CancellationToken cancellationToken)
        {
            if (!new List<string> {PhotoPackageStatuses.Accepted, PhotoPackageStatuses.Rejected}.Contains(
                request.StatusCode))
                throw new ArgumentOutOfRangeException(nameof(request.StatusCode),
                    $"Установка статуса {request.StatusCode} для фотопакета {request.Id} запрещена");

            var photoPackage = await GetPhotoPackage(request.ContractId, request.Id, cancellationToken);

            var processingCompleted = !photoPackage.ProcessCode.Equals(PhotoPackageProcessTypes.DebitOnline) || photoPackage.ProcessCode.Equals(PhotoPackageProcessTypes.DebitOnline) && 
                                      !await ExistsPhotoPackagesWithSameContractId(request.ContractId, request.Id, cancellationToken);
            
            if (!new List<string> {PhotoPackageStatuses.Pending}.Contains(photoPackage.StatusCode))
                throw new ArgumentOutOfRangeException(nameof(photoPackage.StatusCode),
                    $"Проверка пакета фото {photoPackage.Id} в статусе {photoPackage.StatusCode} не предусмотрена");

            if (photoPackage.Photos.Where(p => p.StatusCode.Equals(PhotoStatuses.New) && p.Required)
                .Select(p => p.Id)
                .ToHashSet()
                .Except(request.PhotoErrors.Select(p => p.photoId).ToHashSet()).Any())
                throw new ArgumentOutOfRangeException(nameof(photoPackage.Photos),
                    $"Статус передан не по всем фотографиям");

            photoPackage.StatusCode = request.StatusCode;
            photoPackage.VerifierSapNumber = request.AuthorSapNumber != 0 ? request.AuthorSapNumber : null;
            photoPackage.ErrorCode = request.ErrorCode;
            photoPackage.ErrorMessage = request.ErrorMessage;
            var errorCodes = new List<string>();
            if (!string.IsNullOrEmpty(photoPackage.ErrorCode)) errorCodes.Add(photoPackage.ErrorCode);
            
            foreach (var (photoId, errorCode, errorMessage, statusCode) in request.PhotoErrors)
            {
                if (string.IsNullOrEmpty(statusCode)) throw new ArgumentNullException(nameof(statusCode));
                var photo = photoPackage.Photos.FirstOrDefault(p => p.Id == photoId) ?? throw new ArgumentNullException(nameof(photoId));

                photo.StatusCode = statusCode;
                photo.ErrorCode = errorCode;
                photo.ErrorMessage = errorMessage;
                photo.CanBeRetaken = !string.IsNullOrEmpty(errorCode) || !string.IsNullOrEmpty(photoPackage.ErrorCode);
                
                if (!string.IsNullOrEmpty(photo.ErrorCode)) errorCodes.Add(photo.ErrorCode);
            }

            await _dbContext.SaveChangesAsync(cancellationToken);

            switch (photoPackage.StatusCode)
            {
                case PhotoPackageStatuses.Accepted:
                    await _bus.Publish(new PhotoPackageAcceptedEvent
                    {
                        ContractId = photoPackage.ContractId,
                        Id = photoPackage.Id,
                        AuthorSapNumber = photoPackage.VerifierSapNumber ?? 0,
                        CourierSapNumber = photoPackage.AuthorSapNumber ?? 0,
                        ProcessTypeCode = photoPackage.ProcessCode,
                        TypeCode = photoPackage.TypeCode,
                        ErrorCodes = errorCodes.ToHashSet(),
                        ProcessingCompleted = processingCompleted
                    }, cancellationToken);
                    break;
                case PhotoPackageStatuses.Rejected:
                    await _bus.Publish(new PhotoPackageRejectedEvent
                    {
                        ContractId = photoPackage.ContractId,
                        Id = photoPackage.Id,
                        AuthorSapNumber = photoPackage.VerifierSapNumber ?? 0,
                        CourierSapNumber = photoPackage.AuthorSapNumber ?? 0,
                        ProcessTypeCode = photoPackage.ProcessCode,
                        TypeCode = photoPackage.TypeCode,
                        ErrorCodes = errorCodes.ToHashSet()
                    }, cancellationToken);
                    break;
            }

            if (!processingCompleted)
                await _mediator.Send(new StartPhotoPackageProcessingMCommand
                {
                    ContractId = photoPackage.ContractId
                }, cancellationToken);

            
            return string.Empty;
        }

        private async Task<PhotoPackage> GetPhotoPackage(string contractId, string packagePhotoId,
            CancellationToken cancellationToken)
        {
            return await _dbContext.PhotoPackages
                       .Include(p => p.Photos).IgnoreAutoIncludes()
                       .FirstOrDefaultAsync(pp =>
                           pp.ContractId == contractId && pp.Id == packagePhotoId, cancellationToken) ??
                   throw new ArgumentOutOfRangeException(nameof(packagePhotoId),
                       "Пакет фото с указанным PackagePhotoId не найден");
        }

        private async Task<bool> ExistsPhotoPackagesWithSameContractId(string contractId, string packagePhotoId,
            CancellationToken cancellationToken)
        {
            return await _dbContext.PhotoPackages.AnyAsync(pp => pp.ContractId == contractId && 
                                                                 pp.Id != packagePhotoId,
                cancellationToken);
        }
    }
}