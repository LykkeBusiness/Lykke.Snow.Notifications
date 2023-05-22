using System;
using System.Threading.Tasks;
using Lykke.Snow.Mdm.Contracts.Api;
using Lykke.Snow.Notifications.Domain.Exceptions;
using Lykke.Snow.Notifications.Domain.Model;
using Lykke.Snow.Notifications.Domain.Services;
using Microsoft.Extensions.Internal;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Lykke.Snow.Notifications.DomainServices.Services
{
    public class MdmLocalizationDataProvider : ILocalizationDataProvider
    {
        private LocalizationData? _localizationData;
        private DateTime _lastUpdatedAt;

        private readonly TimeSpan DefaultCacheExpirationPeriod = TimeSpan.FromMinutes(5);
        private readonly ILogger<MdmLocalizationDataProvider> _logger;
        private readonly ILocalizationFilesBinaryApi _localizationFilesBinaryApi;
        private readonly ISystemClock _systemClock;
        private readonly string _localizationPlatformKey;
        private readonly TimeSpan _cacheExpirationPeriod;

        public MdmLocalizationDataProvider(ILogger<MdmLocalizationDataProvider> logger,
            ILocalizationFilesBinaryApi localizationFilesBinaryApi,
            TimeSpan? cacheExpirationPeriod,
            ISystemClock systemClock,
            string localizationPlatformKey)
        {
            if(string.IsNullOrEmpty(localizationPlatformKey))
                throw new ArgumentNullException(nameof(localizationPlatformKey));

            _localizationPlatformKey = localizationPlatformKey;
            _logger = logger;
            _localizationFilesBinaryApi = localizationFilesBinaryApi;
            _cacheExpirationPeriod = cacheExpirationPeriod ?? DefaultCacheExpirationPeriod;
            _systemClock = systemClock;
        }

        public async Task<LocalizationData> Load()
        {
            if(_localizationData == null || _systemClock.UtcNow.DateTime - _lastUpdatedAt > _cacheExpirationPeriod)
            {
                _localizationData = await LoadFromMdm();
                _lastUpdatedAt = _systemClock.UtcNow.DateTime;

                _logger.LogDebug("Localization data has been loaded from Mdm Service and cache has been updated.");
            }   
            
            return _localizationData;
        }
        
        public async Task<LocalizationData> LoadFromMdm()
        {
            var response = await _localizationFilesBinaryApi.GetActiveLocalizationFileAsync(_localizationPlatformKey);
            
            if(!response.IsSuccessStatusCode)
                throw new LocalizationFileCannotBeLoadedException($"The status code was {response.StatusCode} - Key: {_localizationPlatformKey}");

            var jsonText = await response.Content.ReadAsStringAsync();

            try
            {
                var result = JsonConvert.DeserializeObject<LocalizationData>(jsonText);

                if(result == null)
                    throw new LocalizationFileParsingException();

                return result;
            }
            catch(JsonReaderException e)
            {
                _logger.LogError(e, e.Message);

                throw;
            }
            catch(LocalizationFileParsingException e)
            {
                _logger.LogError(e, e.Message);

                throw;
            }
        }
    }
}
