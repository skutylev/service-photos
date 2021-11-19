using System.Collections.Generic;
using Service.PhotoPackages.ServiceLayer.Constants;
using Service.PhotoPackages.ServiceLayer.Integrations.Contracts;
using Service.PhotoPackages.ServiceLayer.Integrations.DMNs.PackagePhotoProcessTypes;
using Service.PhotoPackages.ServiceLayer.Integrations.DMNs.PackagePhotoTypesDmn;
using Service.PhotoPackages.ServiceLayer.Integrations.DMNs.PhotoTypesDmn;
using Service.PhotoPackages.ServiceLayer.Integrations.DMNs.ScanCheckerProcessTypes;
using Service.Settings.Client.Decisions;

namespace Service.PhotoPackages.ServiceLayer.Integrations.DMNs
{
    public class PhotoDmnService : IPhotoDmnService
    {

        private readonly ISettingsDecisionService _settingsDecisionService;

        public PhotoDmnService(ISettingsDecisionService settingsDecisionService)
        {
            _settingsDecisionService = settingsDecisionService;
        }

        public IEnumerable<PackagePhotoTypesDmnResponse> GetPackagePhotoTypes(PackagePhotoTypesDmnRequest request)
        {
            return _settingsDecisionService.GetList<PackagePhotoTypesDmnResponse>(DmnNames.PackagePhotoTypes,
                request.ToDictionary());
        }

        public IEnumerable<PhotoTypesDmnResponse> GetPhotoTypes(PhotoTypesDmnRequest request)
        {
            return _settingsDecisionService.GetList<PhotoTypesDmnResponse>(DmnNames.PhotoTypes, request.ToDictionary());
        }

        public PackagePhotoProcessTypesDmnResponse GetPackagePhotoProcessTypes(PackagePhotoProcessTypesDmnRequest request)
        {
            return _settingsDecisionService.Get<PackagePhotoProcessTypesDmnResponse>(DmnNames.PackagePhotoProcessTypes, request.ToDictionary());
        }

        public ScanCheckerProcessTypesDmnResponse GetScanCheckerProcessTypes(ScanCheckerProcessTypesDmnRequest request)
        {
            return _settingsDecisionService.Get<ScanCheckerProcessTypesDmnResponse>(DmnNames.ScanCheckerProcessTypes, request.ToDictionary());
        }
    }
}