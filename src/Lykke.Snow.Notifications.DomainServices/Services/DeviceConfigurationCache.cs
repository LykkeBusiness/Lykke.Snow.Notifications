using System;
using System.Threading.Tasks;
using Lykke.Snow.Notifications.Domain.Model;
using Lykke.Snow.Notifications.Domain.Repositories;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace Lykke.Snow.Notifications.DomainServices.Services
{
    public class DeviceConfigurationCache : IDeviceConfigurationRepository
    {
        private readonly IDeviceConfigurationRepository _decoratee;
        private readonly IMemoryCache _cache;
        private readonly ILogger<DeviceConfigurationCache> _logger;
        private readonly MemoryCacheEntryOptions _cacheOptions;

        private static TimeSpan DefaultCacheExpirationPeriod => TimeSpan.FromMinutes(10);

        public DeviceConfigurationCache(IDeviceConfigurationRepository decoratee,
            IMemoryCache cache,
            TimeSpan? cacheExpirationPeriod,
            ILogger<DeviceConfigurationCache> logger)
        {
            _decoratee = decoratee;
            _cache = cache;
            _logger = logger;
            _cacheOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(cacheExpirationPeriod ?? DefaultCacheExpirationPeriod);
        }

        public async Task<DeviceConfiguration> GetAsync(string deviceId, string accountId)
        {
            var cacheKey = GetCacheKey(deviceId, accountId);

            if (_cache.TryGetValue(cacheKey, out DeviceConfiguration? deviceConfiguration))
                return deviceConfiguration!;

            deviceConfiguration = await _decoratee.GetAsync(deviceId, accountId);

            _cache.Set(cacheKey, deviceConfiguration, _cacheOptions);
            _logger.LogDebug("Device configuration with key {Key} was added to cache", cacheKey);

            return deviceConfiguration;
        }

        public async Task AddOrUpdateAsync(DeviceConfiguration deviceConfiguration)
        {
            await _decoratee.AddOrUpdateAsync(deviceConfiguration);

            var cacheKey = GetCacheKey(deviceConfiguration.DeviceId, deviceConfiguration.AccountId);
            _cache.Remove(cacheKey);
            _logger.LogDebug("Device configuration with key {Key} was removed from cache due to add or update",
                cacheKey);
        }

        public async Task RemoveAsync(string deviceId, string accountId)
        {
            await _decoratee.RemoveAsync(deviceId, accountId);

            var cacheKey = GetCacheKey(deviceId, accountId);
            _cache.Remove(cacheKey);
            _logger.LogDebug("Device configuration with key {Key} was removed from cache", cacheKey);
        }

        private static string GetCacheKey(string deviceId, string accountId) => $"DeviceConfiguration_{deviceId}_{accountId}";

    }
}
