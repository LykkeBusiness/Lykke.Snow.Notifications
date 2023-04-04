using System;
using System.Threading.Tasks;
using AutoMapper;
using Lykke.Snow.Notifications.Domain.Exceptions;
using Lykke.Snow.Notifications.Domain.Model;
using Lykke.Snow.Notifications.SqlRepositories.Entities;
using Lykke.Snow.Notifications.SqlRepositories.Repositories;
using Lykke.Snow.Notifications.Tests.Fakes;
using Moq;
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

        private DeviceRegistrationRepository CreateSut()
        {
            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<DeviceRegistration>(It.IsAny<DeviceRegistrationEntity>()))
                .Returns(new DeviceRegistration("account-id", "device-token", "device-id", DateTime.UtcNow));
            return new DeviceRegistrationRepository(new MssqlContextFactoryFake(), new Mock<IMapper>().Object);
        }
    }
}
