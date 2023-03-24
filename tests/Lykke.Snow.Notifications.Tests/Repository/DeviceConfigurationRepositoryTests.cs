using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Lykke.Snow.Notifications.Domain.Exceptions;
using Lykke.Snow.Notifications.Domain.Model;
using Lykke.Snow.Notifications.MappingProfiles;
using Lykke.Snow.Notifications.SqlRepositories.Entities;
using Lykke.Snow.Notifications.SqlRepositories.Repositories;
using Lykke.Snow.Notifications.Tests.Fakes;
using Xunit;

namespace Lykke.Snow.Notifications.Tests.Repository
{
    public class DeviceConfigurationRepositoryTests
    {
        private readonly IMapper _mapper;

        public DeviceConfigurationRepositoryTests()
        {
            _mapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<MappingProfile>();
            }).CreateMapper();
        }

        [Fact]
        public async Task GetAsync_Should_Return_DeviceConfiguration_When_Exists()
        {
            // Arrange
            var repo = new DeviceConfigurationRepository(new MssqlContextFactoryFake(), _mapper);
            await SeedDatabaseAsync("test-device-1", "test-account-1");

            // Act
            var deviceConfiguration = await repo.GetAsync("test-device-1");

            // Assert
            Assert.NotNull(deviceConfiguration);
        }
        
        [Fact]
        public async Task GetAsync_Should_Throw_EntityNotFoundException_When_Not_Exists()
        {
            // Arrange
            var repo = new DeviceConfigurationRepository(new MssqlContextFactoryFake(), _mapper);

            // Act & Assert
            await Assert.ThrowsAsync<EntityNotFoundException>(() => repo.GetAsync("test-device-2"));
        }

        [Fact]
        public async Task AddOrUpdateAsync_Should_Add_New_DeviceConfiguration()
        {
            // Arrange
            var repo = new DeviceConfigurationRepository(new MssqlContextFactoryFake(), _mapper);
            var deviceConfiguration = DeviceConfiguration.Default("device-id-3", "account-id-3");

            // Act
            await repo.AddOrUpdateAsync(deviceConfiguration);

            // Assert
            var addedDeviceConfiguration = await repo.GetAsync(deviceConfiguration.DeviceId);
            Assert.NotNull(addedDeviceConfiguration);
        }

        [Fact]
        public async Task AddOrUpdateAsync_Should_Update_Existing_DeviceConfiguration()
        {
            // Arrange
            const string deviceId = "test-device-4";
            var repo = new DeviceConfigurationRepository(new MssqlContextFactoryFake(), _mapper);
            await SeedDatabaseAsync(deviceId, "test-account-4", "fr");

            var updatedDeviceConfiguration = new DeviceConfiguration(deviceId,
                "test-account-updated-4",
                locale: "fr-updated",
                notifications: new List<DeviceConfiguration.Notification>
                {
                    new DeviceConfiguration.Notification("DepositSucceeded")
                });

            // Act
            await repo.AddOrUpdateAsync(updatedDeviceConfiguration);

            // Assert
            var fetchedDeviceConfiguration = await repo.GetAsync(updatedDeviceConfiguration.DeviceId);
            Assert.NotNull(fetchedDeviceConfiguration);
            Assert.Equal(updatedDeviceConfiguration.AccountId, fetchedDeviceConfiguration.AccountId);
            Assert.Equal(updatedDeviceConfiguration.Locale, fetchedDeviceConfiguration.Locale);
            Assert.Equal(updatedDeviceConfiguration.Notifications.Count,
                fetchedDeviceConfiguration.Notifications.Count);
            Assert.True(fetchedDeviceConfiguration.IsNotificationEnabled("DepositSucceeded"));
        }

        [Fact]
        public async Task RemoveAsync_Should_Remove_DeviceConfiguration_When_Exists()
        {
            // Arrange
            const string deviceId = "test-device-5";
            var repo = new DeviceConfigurationRepository(new MssqlContextFactoryFake(), _mapper);
            await SeedDatabaseAsync(deviceId, "test-account-5");

            // Act
            await repo.RemoveAsync(deviceId);

            // Assert
            await Assert.ThrowsAsync<EntityNotFoundException>(() => repo.GetAsync(deviceId));
        }

        [Fact]
        public async Task RemoveAsync_Should_Throw_EntityNotFoundException_When_Not_Exists()
        {
            // Arrange
            var repo = new DeviceConfigurationRepository(new MssqlContextFactoryFake(), _mapper);

            // Act & Assert
            await Assert.ThrowsAsync<EntityNotFoundException>(() => repo.RemoveAsync("test-device-6"));
        }
        
        private async Task SeedDatabaseAsync(string deviceId, string accountId, string locale = "en")
        {
            await using var context = new MssqlContextFactoryFake().CreateDataContext();

            context.DeviceConfigurations.Add(new DeviceConfigurationEntity
            {
                DeviceId = deviceId, 
                AccountId = accountId, 
                Locale = locale
            });
            
            await context.SaveChangesAsync();
        }
    }
}
