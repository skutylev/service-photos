using System.Collections.Generic;
using Service.PhotoPackages.ServiceLayer.Integrations.DMNs.PackagePhotoProcessTypes;
using Service.PhotoPackages.ServiceLayer.Integrations.DMNs.PackagePhotoTypesDmn;
using Service.PhotoPackages.ServiceLayer.Integrations.DMNs.PhotoTypesDmn;
using Service.PhotoPackages.ServiceLayer.Integrations.DMNs.ScanCheckerProcessTypes;

namespace Service.PhotoPackages.ServiceLayer.Integrations.Contracts
{
    public interface IPhotoDmnService
    {
        IEnumerable<PackagePhotoTypesDmnResponse> GetPackagePhotoTypes(PackagePhotoTypesDmnRequest request);
        IEnumerable<PhotoTypesDmnResponse> GetPhotoTypes(PhotoTypesDmnRequest request);
        PackagePhotoProcessTypesDmnResponse GetPackagePhotoProcessTypes(PackagePhotoProcessTypesDmnRequest request);
        ScanCheckerProcessTypesDmnResponse GetScanCheckerProcessTypes(ScanCheckerProcessTypesDmnRequest request);
    }
}