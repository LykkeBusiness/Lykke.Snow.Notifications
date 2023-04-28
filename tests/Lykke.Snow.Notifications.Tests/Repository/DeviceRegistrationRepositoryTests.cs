using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Lykke.Snow.Notifications.Domain.Model;
using Lykke.Snow.Notifications.MappingProfiles;
using Lykke.Snow.Notifications.SqlRepositories.Repositories;
using Lykke.Snow.Notifications.Tests.Fakes;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Lykke.Snow.Notifications.Tests.Repository
{
    public class DeviceRegistrationRepositoryTests
    {
        [Fact]
        public async Task GetDeviceRegistrationAsync_ShouldReturnEmptyCollection_WhenPassedNonExistentDeviceToken()
        {
            var sut = CreateSut();
            var result = await sut.GetDeviceRegistrationsAsync("device-token-that-does-not-exist-on-database");
            
            Assert.Empty(result);
        }

        [Theory]
        [InlineData(2)]
        [InlineData(10)]
        [InlineData(100)]
        public async Task AddOrUpdateAsync_RaceCondition_NotPossible(int numberOfTasks)
        {
            // Arrange
            var sut = CreateSut();

            var deviceRegistration = new DeviceRegistration("account-id", "device-token", "device-id", DateTime.UtcNow);
            
            // Act
            var tasks = new List<Task>();
            for (var i = 0; i < numberOfTasks; i++)
            {
                tasks.Add(Task.Run(async () => await sut.AddOrUpdateAsync(deviceRegistration)));
            }

            await Task.WhenAll(tasks);

            // Assert
            await using var context = new MsSqlContextFactoryInMemory().CreateDataContext();
            var actualCount = await context.DeviceRegistrations.CountAsync(x =>
                x.DeviceToken == deviceRegistration.DeviceToken && x.AccountId == deviceRegistration.AccountId);
                
            Assert.Equal(1, actualCount);
        }

        private DeviceRegistrationRepository CreateSut()
        {
            var mapper = new MapperConfiguration(
                    cfg => cfg.AddProfile(new MappingProfile()))
                .CreateMapper();
            
            return new DeviceRegistrationRepository(new MsSqlContextFactoryInMemory(), mapper);
        }
    }
}
