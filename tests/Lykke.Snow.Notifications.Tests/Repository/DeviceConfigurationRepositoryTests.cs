﻿using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Lykke.Snow.Notifications.Domain.Exceptions;
using Lykke.Snow.Notifications.Domain.Enums;
using Lykke.Snow.Notifications.Domain.Model;
using Lykke.Snow.Notifications.MappingProfiles;
using Lykke.Snow.Notifications.SqlRepositories.Entities;
using Lykke.Snow.Notifications.SqlRepositories.Repositories;
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
            var repo = new DeviceConfigurationRepository(new MsSqlContextFactoryInMemory(), _mapper);
            await SeedDatabaseAsync("test-device-1", "test-account-1");

            // Act
            var deviceConfiguration = await repo.GetAsync("test-device-1", "test-account-1");

            // Assert
            Assert.NotNull(deviceConfiguration);
        }
        
        [Fact]
        public async Task GetAsync_ShouldReturnNull_EntityNotFoundException_When_Not_Exists()
        {
            // Arrange
            var repo = new DeviceConfigurationRepository(new MsSqlContextFactoryInMemory(), _mapper);

            // Act & Assert
            var result = await repo.GetAsync("test-device-2", "test-account-2");
            Assert.Null(result);
        }

        [Fact]
        public async Task AddOrUpdateAsync_Should_Add_New_DeviceConfiguration()
        {
            // Arrange
            var repo = new DeviceConfigurationRepository(new MsSqlContextFactoryInMemory(), _mapper);
            var deviceConfiguration = DeviceConfiguration.Default("device-id-3", "account-id-3");

            // Act
            await repo.AddOrUpdateAsync(deviceConfiguration);

            // Assert
            var addedDeviceConfiguration = await repo.GetAsync(deviceConfiguration.DeviceId, deviceConfiguration.AccountId);
            Assert.NotNull(addedDeviceConfiguration);
        }

        [Fact]
        public async Task AddOrUpdateAsync_Should_Update_Existing_DeviceConfiguration()
        {
            // Arrange
            const string deviceId = "test-device-4";
            const string accountId = "test-device-4";

            var repo = new DeviceConfigurationRepository(new MsSqlContextFactoryInMemory(), _mapper);
            await SeedDatabaseAsync(deviceId, "test-account-4", Locale.De.ToString());

            var updatedDeviceConfiguration = new DeviceConfiguration(deviceId,
                accountId,
                locale: Locale.Es.ToString(),
                notifications: new List<DeviceConfiguration.Notification>
                {
                    new DeviceConfiguration.Notification("DepositSucceeded")
                });

            // Act
            await repo.AddOrUpdateAsync(updatedDeviceConfiguration);

            // Assert
            var fetchedDeviceConfiguration = await repo.GetAsync(updatedDeviceConfiguration.DeviceId, updatedDeviceConfiguration.AccountId);
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
            const string accountId = "test-account-5";

            var repo = new DeviceConfigurationRepository(new MsSqlContextFactoryInMemory(), _mapper);
            await SeedDatabaseAsync(deviceId, accountId);

            // Act
            await repo.RemoveAsync(deviceId, accountId);

            // Assert
            var result = await repo.GetAsync(deviceId, accountId);

            Assert.Null(result);
        }
        
        [Fact]
        public async Task AddOrUpdateAsync_MultipleConfigurations_For_Same_Device_Should_Be_Possible()
        {
            // Arrange
            var repo = new DeviceConfigurationRepository(new MsSqlContextFactoryInMemory(), _mapper);
            var dcAccountA = DeviceConfiguration.Default("device-id-6", "account-id-a");
            var dcAccountB = new DeviceConfiguration("device-id-6",
                "account-id-b",
                "de",
                new List<DeviceConfiguration.Notification> { new DeviceConfiguration.Notification("AccountLocked") });

            // Act
            await repo.AddOrUpdateAsync(dcAccountA);
            await repo.AddOrUpdateAsync(dcAccountB);

            // Assert
            var addedA = await repo.GetAsync(dcAccountA.DeviceId, dcAccountA.AccountId);
            Assert.Equal(dcAccountA, addedA);
            var addedB = await repo.GetAsync(dcAccountB.DeviceId, dcAccountB.AccountId);
            Assert.Equal(dcAccountB, addedB);
        }

        [Fact]
        public async Task RemoveAsync_Should_Throw_EntityNotFoundException_When_Not_Exists()
        {
            // Arrange
            var repo = new DeviceConfigurationRepository(new MsSqlContextFactoryInMemory(), _mapper);

            // Act & Assert
            await Assert.ThrowsAsync<EntityNotFoundException>(() => repo.RemoveAsync("test-device-6", "test-account-6"));
        }
        
        private async Task SeedDatabaseAsync(string deviceId, string accountId, string locale = "en")
        {
            await using var context = new MsSqlContextFactoryInMemory().CreateDataContext();

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
