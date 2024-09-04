using System;
using System.Threading.Tasks;
using Lykke.Snow.Mdm.Contracts.Api;
using Lykke.Snow.Notifications.Domain.Exceptions;
using Lykke.Snow.Notifications.Domain.Model;
using Lykke.Snow.Notifications.Domain.Services;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Lykke.Snow.Notifications.DomainServices.Services
{
    public class MdmLocalizationDataProvider : ILocalizationDataProvider
    {
        private readonly TimeSpan DefaultCacheExpirationPeriod = TimeSpan.FromMinutes(5);
        private readonly ILogger<MdmLocalizationDataProvider> _logger;
        private readonly ILocalizationFilesBinaryApi _localizationFilesBinaryApi;
        private readonly string _localizationPlatformKey;
        private const string _cacheKey = "LocalizationFile_Mdm";
        private readonly MemoryCacheEntryOptions _cacheOptions;
        private readonly IMemoryCache _cache;

        public MdmLocalizationDataProvider(ILogger<MdmLocalizationDataProvider> logger,
            ILocalizationFilesBinaryApi localizationFilesBinaryApi,
            TimeSpan? cacheExpirationPeriod,
            string localizationPlatformKey,
            IMemoryCache cache)
        {
            if (string.IsNullOrEmpty(localizationPlatformKey))
                throw new ArgumentNullException(nameof(localizationPlatformKey));

            _localizationPlatformKey = localizationPlatformKey;
            _logger = logger;
            _localizationFilesBinaryApi = localizationFilesBinaryApi;
            _cache = cache;
            _cacheOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(cacheExpirationPeriod ?? DefaultCacheExpirationPeriod);
        }

        public async Task<LocalizationData> Load()
        {
            if (_cache.TryGetValue(_cacheKey, out LocalizationData? localizationData))
                return localizationData!;

            localizationData = await LoadFromMdm();

            _cache.Set(_cacheKey, localizationData, _cacheOptions);

            _logger.LogDebug("Localization data has been loaded from Mdm Service and cache has been updated.");

            return localizationData;
        }

        public async Task<LocalizationData> LoadFromMdm()
        {
            var response = await _localizationFilesBinaryApi.GetActiveLocalizationFileAsync(_localizationPlatformKey);

            if (!response.IsSuccessStatusCode)
                throw new LocalizationFileCannotBeLoadedException($"The status code was {response.StatusCode} - Key: {_localizationPlatformKey}");

            var jsonText = await response.Content.ReadAsStringAsync();

            try
            {
                var result = JsonConvert.DeserializeObject<LocalizationData>(jsonText);

                if (result == null)
                    throw new LocalizationFileParsingException();

                return result;
            }
            catch (JsonReaderException e)
            {
                _logger.LogError(e, "Could not parse the json string into localization data.");

                throw;
            }
            catch (LocalizationFileParsingException e)
            {
                _logger.LogError(e, "Could not parse the json string into localization data.");

                throw;
            }
        }
    }
}
