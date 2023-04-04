using System;
using System.Threading.Tasks;
using Lykke.Snow.Notifications.Domain.Model;
using Lykke.Snow.Notifications.Domain.Repositories;
using Lykke.Snow.Notifications.DomainServices.Services;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Lykke.Snow.Notifications.Tests.Repository
{
    public class DeviceConfigurationCacheTests
    {
        private readonly Mock<IDeviceConfigurationRepository> _decorateeMock;
        private readonly IMemoryCache _memoryCache;
        private readonly ILogger<DeviceConfigurationCache> _logger;
        
        public DeviceConfigurationCacheTests()
        {
            _decorateeMock = new Mock<IDeviceConfigurationRepository>();
            _memoryCache = new MemoryCache(new MemoryCacheOptions());
            _logger = LoggerFactory
                .Create(builder => builder.AddConsole())
                .CreateLogger<DeviceConfigurationCache>();
        }
        
        [Fact]
        public async Task GetAsync_Should_Cache_And_Return_DeviceConfiguration()
        {
            // Arrange
            const string deviceId = "test-device-1";
            const string accountId = "test-account-1";

            var deviceConfiguration = new DeviceConfiguration(deviceId, accountId);

            _decorateeMock.Setup(x => x.GetAsync(deviceId, accountId)).ReturnsAsync(deviceConfiguration);
            var cache = new DeviceConfigurationCache(_decorateeMock.Object, _memoryCache, TimeSpan.FromMinutes(1), _logger);

            // Act
            var result1 = await cache.GetAsync(deviceId, accountId);
            var result2 = await cache.GetAsync(deviceId, accountId);

            // Assert
            Assert.Equal(deviceConfiguration, result1);
            Assert.Equal(deviceConfiguration, result2);
            _decorateeMock.Verify(x => x.GetAsync(deviceId, accountId), Times.Once);
        }
        
        [Fact]
        public async Task AddOrUpdateAsync_Should_Invalidate_Cache()
        {
            // Arrange
            const string deviceId = "test-device-2";
            const string accountId = "test-account-2";

            var deviceConfiguration = new DeviceConfiguration(deviceId, accountId);
            _decorateeMock.Setup(x => x.GetAsync(deviceId, accountId)).ReturnsAsync(deviceConfiguration);
            var cache = new DeviceConfigurationCache(_decorateeMock.Object, _memoryCache, TimeSpan.FromMinutes(1),  _logger);

            // Act
            await cache.AddOrUpdateAsync(deviceConfiguration);
            await cache.GetAsync(deviceId, accountId);

            // Assert
            _decorateeMock.Verify(x => x.GetAsync(deviceId, accountId), Times.Exactly(2));
        }
        
        [Fact]
        public async Task RemoveAsync_Should_Invalidate_Cache()
        {
            // Arrange
            const string deviceId = "test-device-3";
            const string accountId = "test-account-3";

            var deviceConfiguration = new DeviceConfiguration(deviceId, accountId);
            _decorateeMock.Setup(x => x.GetAsync(deviceId, accountId)).ReturnsAsync(deviceConfiguration);
            var cache = new DeviceConfigurationCache(_decorateeMock.Object, _memoryCache, TimeSpan.FromMinutes(1),  _logger);

            // Act
            await cache.GetAsync(deviceId, accountId);
            await cache.RemoveAsync(deviceId, accountId);
            await cache.GetAsync(deviceId, accountId);

            // Assert
            _decorateeMock.Verify(x => x.GetAsync(deviceId, accountId), Times.Exactly(2));
        }
        
        [Fact]
        public async Task GetAsync_Should_Fetch_From_Decoratee_If_Cache_Invalidates()
        {
            // Arrange
            const string deviceId = "test-device-4";
            const string accountId = "test-device-4";

            const int cacheExpirationInSeconds = 3;
            var deviceConfiguration = new DeviceConfiguration(deviceId, accountId);

            _decorateeMock.Setup(x => x.GetAsync(deviceId, accountId)).ReturnsAsync(deviceConfiguration);
            var cache = new DeviceConfigurationCache(_decorateeMock.Object,
                _memoryCache,
                TimeSpan.FromSeconds(cacheExpirationInSeconds),
                _logger);

            // Act
            await cache.GetAsync(deviceId, accountId);
            await Task.Delay(TimeSpan.FromSeconds(cacheExpirationInSeconds + 1));
            await cache.GetAsync(deviceId, accountId);

            // Assert
            _decorateeMock.Verify(x => x.GetAsync(deviceId, accountId), Times.Exactly(2));
        }
    }
}
