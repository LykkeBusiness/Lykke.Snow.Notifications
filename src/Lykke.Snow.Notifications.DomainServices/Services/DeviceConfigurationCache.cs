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

        public DeviceConfigurationCache(IDeviceConfigurationRepository decoratee,
            IMemoryCache cache,
            ILogger<DeviceConfigurationCache> logger)
        {
            _decoratee = decoratee;
            _cache = cache;
            _logger = logger;
        }

        public async Task<DeviceConfiguration> GetAsync(string deviceId)
        {
            var cacheKey = GetCacheKey(deviceId);

            if (_cache.TryGetValue(cacheKey, out DeviceConfiguration deviceConfiguration))
                return deviceConfiguration;

            deviceConfiguration = await _decoratee.GetAsync(deviceId);

            _cache.Set(cacheKey, deviceConfiguration);
            _logger.LogDebug("Device configuration with key {Key} was added to cache", cacheKey);

            return deviceConfiguration;
        }

        public async Task AddOrUpdateAsync(DeviceConfiguration deviceConfiguration)
        {
            await _decoratee.AddOrUpdateAsync(deviceConfiguration);

            var cacheKey = GetCacheKey(deviceConfiguration.DeviceId);
            _cache.Remove(cacheKey);
            _logger.LogDebug("Device configuration with key {Key} was removed from cache due to add or update", cacheKey);
        }

        public async Task RemoveAsync(string deviceId)
        {
            await _decoratee.RemoveAsync(deviceId);

            var cacheKey = GetCacheKey(deviceId);
            _cache.Remove(cacheKey);
            _logger.LogDebug("Device configuration with key {Key} was removed from cache", cacheKey);
        }

        private static string GetCacheKey(string deviceId) => $"DeviceConfiguration_{deviceId}";
    }
}