using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Service.PhotoPackages.ServiceLayer.MediatR.Commands.ChangePhotoTypesRequirementsForExUnderage;
using Service.PhotoPackages.ServiceLayer.MediatR.Commands.CheckPhotoPackageProcessing;
using Service.PhotoPackages.ServiceLayer.MediatR.Commands.ProcessScanCheckerResults;
using Service.PhotoPackages.ServiceLayer.Migrator;

namespace Service.PhotoPackages.Controllers
{
    [ApiController, ApiVersion("1"), Produces("application/json")]
    [Route("v1/tasks")]
    public class TasksController : ControllerBase
    {
        [HttpPost("check-scan-checker-results")]
        public async Task<bool> CheckScanCheckerResults([FromServices] IMediator mediator)
        {
            await mediator.Send(new ProcessScanCheckerResultsMCommand());
            await mediator.Send(new CheckPhotoPackagesProcessingMCommand());
            return true;
        }
        
        [HttpPost("change-photo-types-requirement-for-exunderage")]
        public async Task<bool> ChangePhotoTypesRequirementForExUnderAge([FromServices] IMediator mediator)
        {
            await mediator.Send(new ChangePhotoTypesRequirementsForExUnderageMCommand());
            return true;
        }
        
        [HttpPost("migrate-photo-packages")]
        public async Task<bool> MigratePhotoPackages([FromServices] IMigratorService service, [FromQuery] List<long> packageIds, [FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
        {
            await service.MigratePhotoPackages(packageIds, startDate, endDate);
            return true;
        }
    }
}