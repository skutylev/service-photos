using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MassTransit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Service.Contracts.Events.Reports;
using Service.PhotoPackages.Dal;
using Service.PhotoPackages.Dal.Models;
using Service.PhotoPackages.ServiceLayer.Constants;
using Service.PhotoPackages.ServiceLayer.Integrations.Contracts;

namespace Service.PhotoPackages.ServiceLayer.MediatR.Commands.SendPhotoDocumentsOnEmail
{
    public class SendPhotoDocumentsOnEmailMCommand : IRequest
    {
        public string ContractId { get; set; }
        public long SapNumber { get; set; }
    }

    public class SendPhotoDocumentsOnEmailMCommandHandler : AsyncRequestHandler<SendPhotoDocumentsOnEmailMCommand>
    {
        private readonly PhotoPackagesDbContext _dbContext;
        private readonly ISettingsService _settingsService;
        private readonly IUsersService _usersService;
        private readonly IBus _bus;

        public SendPhotoDocumentsOnEmailMCommandHandler(PhotoPackagesDbContext dbContext,
            ISettingsService settingsService,
            IUsersService usersService, IBus bus)
        {
            _dbContext = dbContext;
            _settingsService = settingsService;
            _usersService = usersService;
            _bus = bus;
        }

        protected override async Task Handle(SendPhotoDocumentsOnEmailMCommand request,
            CancellationToken cancellationToken)
        {
            var sendingPhotoTypes = _settingsService.GetPhotoTypes().Where(p =>
                p.Properties != null &&    
                p.Properties.Any(pp => pp.Name.Equals(PhotoTypeProperties.IsSentToEmail, StringComparison.InvariantCultureIgnoreCase)))
                .Select(p => new
            {
                p.Code,
                GroupCode =  p.Properties.FirstOrDefault(pr => pr.Name == PhotoTypeProperties.PhotoGroupCode)?.Value ?? p.Code,
                Format = p.Properties.FirstOrDefault(pr => pr.Name == PhotoTypeProperties.MobileDocumentFormat)?.Value ?? "A4",
            }).ToList();
            
            var photoPackages = await GetPhotoPackages(request.ContractId, cancellationToken);
            if (!photoPackages.All(p => p.StatusCode.Equals(PhotoPackageStatuses.Accepted)))
                throw new ArgumentOutOfRangeException(nameof(photoPackages), $"Не возможно отправка фото документов фотопакетов в статусах отличных от {PhotoPackageStatuses.Accepted}");
            
            if (photoPackages.Any(pp => pp.Photos
                    .Where(p => p.Required && sendingPhotoTypes.Select(t => t.Code).Contains(p.TypeCode, StringComparer.InvariantCultureIgnoreCase))
                    .All(p => !p.StatusCode.Equals(PhotoStatuses.Accepted, StringComparison.InvariantCultureIgnoreCase) && string.IsNullOrEmpty(p.ContentId))))
                throw new ArgumentOutOfRangeException(nameof(photoPackages), $"Не все фотографии, необходимые для отправки на почту, были приняты или приложены к пакету фотографий");

            var user = (await _usersService.GetUsers(new[] {request.SapNumber}, cancellationToken))
                .FirstOrDefault() ?? throw new ArgumentNullException(nameof(request.SapNumber), $"Пользователь не найден в системе");
            
            if (string.IsNullOrEmpty(user.Email))
                throw new ArgumentNullException(nameof(user.Email), $"По выбранному пользователю не указан e-mail");

            var photoInfos = photoPackages.SelectMany(package =>
                package.Photos
                    .Where(photo => sendingPhotoTypes.Select(t => t.Code)
                        .Contains(photo.TypeCode, StringComparer.InvariantCultureIgnoreCase))
                    .Select(photo => new {
                            TypeCode = package.TypeCode,
                            ContentId = photo.ContentId,
                            PhotoGroupTypeCode = sendingPhotoTypes.FirstOrDefault(t => t.Code.Equals(photo.TypeCode, StringComparison.InvariantCultureIgnoreCase))?.GroupCode ?? photo.TypeCode,
                            Format = sendingPhotoTypes.FirstOrDefault(t => t.Code.Equals(photo.TypeCode, StringComparison.InvariantCultureIgnoreCase))?.Format ?? "A4"
                        }
                    ));

            await _bus.Publish(new CreatePassportReportCommand
            {
                ContractId = request.ContractId,
                SapNumber = user.SapNumber,
                Email = user.Email,
                Photos = photoInfos.Select(i => new PhotoForPrint
                {
                    Format = i.Format,
                    PhotoGroupCode = i.PhotoGroupTypeCode,
                    PhotoPackageTypeCode = i.TypeCode,
                    ContentId = i.ContentId
                })
            }, cancellationToken);
        }
        
        private async Task<List<PhotoPackage>> GetPhotoPackages(string contractId, CancellationToken cancellationToken)
        {
            return await _dbContext.PhotoPackages
                .Include(p => p.Photos)
                .Where(p => p.ContractId == contractId && p.TypeCode == PhotoPackageTypes.Contract)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }
    }
}