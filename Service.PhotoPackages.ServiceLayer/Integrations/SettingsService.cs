using System;
using System.Collections.Generic;
using System.Linq;
using Service.PhotoPackages.ServiceLayer.Constants;
using Service.PhotoPackages.ServiceLayer.Integrations.Contracts;
using Service.Settings.Client.Contracts.Dto;
using Service.Settings.Client.Entities;
using Service.Settings.Client.Parameters;
using Service.Settings.Client.Settings;

namespace Service.PhotoPackages.ServiceLayer.Integrations
{
    public class SettingsService : ISettingsService
    {
        private readonly IGetSettingsService _getSettingsService;
        private readonly IGetParametersService _getParametersService;

        public SettingsService(IGetSettingsService getSettingsService, IGetParametersService getParametersService)
        {
            _getSettingsService = getSettingsService;
            _getParametersService = getParametersService;
        }

        public IEnumerable<DictionaryItemDto> GetPhotoPackageTypes()
        {
            return _getSettingsService
                .Get(DictionariesNames.PhotoPackageTypes)
                .Select(s => GetDictionaryItemDto(s));
        }

        public IEnumerable<DictionaryItemDto> GetPhotoStatuses()
        {
            return _getSettingsService
                .Get(DictionariesNames.PhotoStatuses)
                .Select(s => GetDictionaryItemDto(s));
        }

        public IEnumerable<DictionaryItemDto> GetPhotoPackageProcessTypes()
        {
            return _getSettingsService
                .Get(DictionariesNames.PhotoPackageProcessTypes)
                .Select(s => GetDictionaryItemDto(s));
        }

        public IEnumerable<DictionaryItemDto> GetPhotoPackageStatuses()
        {
            return _getSettingsService
                .Get(DictionariesNames.PhotoPackageStatuses)
                .Select(s => GetDictionaryItemDto(s));
        }

        public IEnumerable<DictionaryItemDto> GetPhotoGroupTypes()
        {
            return _getSettingsService
                .Get(DictionariesNames.PhotoGroupTypes)
                .Select(s => GetDictionaryItemDto(s));
        }

        public IEnumerable<DictionaryItemDto> GetPhotoErrorTypes()
        {
            return _getSettingsService
                .Get(DictionariesNames.PhotoErrorTypes)
                .Select(s => GetDictionaryItemDto(s));
        }

        public IEnumerable<DictionaryItemDto> GetPhotoTypes(string callerType = null)
        {
            return _getSettingsService
                .Get(DictionariesNames.PhotoTypes)
                .Select(s => GetDictionaryItemDto(s, callerType));
         }

        public DateTimeOffset GetDateTimeOffsetFromUtcTime(DateTime utcDateTime, int utcOffset)
        {
            var dateTimeOffset = new DateTimeOffset(DateTime.SpecifyKind(utcDateTime, DateTimeKind.Utc));
            return dateTimeOffset.ToOffset(TimeSpan.FromHours(utcOffset));
        }

        public DictionaryItemDto GetPhotoGroupType(string photoTypeCode)
        {
            var property = _getSettingsService
                .Get(DictionariesNames.PhotoTypes)
                .FirstOrDefault(s => s.Code == photoTypeCode)
                ?.Properties
                ?.FirstOrDefault(s => s.Name == PhotoTypeProperties.PhotoGroupCode);
            if (property == null) return GetPhotoTypes().FirstOrDefault(p => p.Code == photoTypeCode);
            var groupDict = GetPhotoGroupTypes().FirstOrDefault(s => s.Code == property.Value);
            return groupDict ?? GetPhotoTypes().FirstOrDefault(p => p.Code == photoTypeCode);
        }

        public string GetBaseUriForContent(string callerType)
        {
            return callerType switch
            {
                CallerTypes.Mobile => _getParametersService.GetByKey(ParameterNames.BaseContentUrlMobile).Value,
                CallerTypes.Web => _getParametersService.GetByKey(ParameterNames.BaseContentUrlWeb).Value,
                _ => "https://localhost"
            };
        }

        public string GetUviaBaseUrl()
        {
            return _getParametersService.GetByKey(ParameterNames.BaseUviaUrlWeb)?.Value ?? "http://localhost";
        }

        public IEnumerable<string> GetUviaEmails()
        {
            var uviaEmails = _getParametersService.GetByKey(ParameterNames.UviaEmail);
            return uviaEmails == null ? new List<string>() : uviaEmails.Value.Split(',').Select(s => s.Trim(' ')).ToList();
        }

        public string GetBaseDocumentReportUrl()
        {
            return _getParametersService.GetByKey(ParameterNames.BaseDocumentReportUrl)?.Value ?? string.Empty;
        }

        private DictionaryItemDto GetDictionaryItemDto(Setting setting, string callerType = null)
        {
            return setting != null
                ? new DictionaryItemDto
                {
                    Code = setting.Code,
                    Flag = setting.Flag,
                    Name = setting.Name,
                    Properties = setting.Properties != null ? (!string.IsNullOrEmpty(callerType) 
                        ?  setting.Properties.Where(p =>
                            p.Name.StartsWith(callerType, StringComparison.InvariantCultureIgnoreCase)) : setting.Properties) : null
                }
                : null;
        }
    }
}