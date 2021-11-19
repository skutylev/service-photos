using System;
using System.Collections.Generic;
using Service.Settings.Client.Contracts.Dto;

namespace Service.PhotoPackages.ServiceLayer.Integrations.Contracts
{
    public interface ISettingsService
    {
        public IEnumerable<DictionaryItemDto> GetPhotoPackageTypes();
        public IEnumerable<DictionaryItemDto> GetPhotoStatuses();
        public IEnumerable<DictionaryItemDto> GetPhotoPackageProcessTypes();
        public IEnumerable<DictionaryItemDto> GetPhotoPackageStatuses();
        public IEnumerable<DictionaryItemDto> GetPhotoGroupTypes();
        public IEnumerable<DictionaryItemDto> GetPhotoErrorTypes();
        IEnumerable<DictionaryItemDto> GetPhotoTypes(string callerType = null);
        public DateTimeOffset GetDateTimeOffsetFromUtcTime(DateTime utcDateTime, int utcOffset);
        public DictionaryItemDto GetPhotoGroupType(string photoTypeCode);
        public string GetBaseUriForContent(string callerType);

        public string GetUviaBaseUrl();
        IEnumerable<string> GetUviaEmails();
        public string GetBaseDocumentReportUrl();

    }
}