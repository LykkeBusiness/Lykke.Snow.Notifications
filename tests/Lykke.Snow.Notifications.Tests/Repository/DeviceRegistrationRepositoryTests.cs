using System.Threading.Tasks;
using AutoMapper;
using Lykke.Snow.Notifications.Domain.Enums;
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
        public async Task GetDeviceRegistrationAsync_ShouldReturnDoesntExist_WhenResultIsNull()
        {
            var sut = CreateSut();
            
            var result = await sut.GetDeviceRegistrationAsync("device-token-that-does-not-exist-on-database");
            
            Assert.True(result.IsFailed);
            Assert.False(result.IsSuccess);
            Assert.Equal(DeviceRegistrationErrorCode.DoesNotExist, result.Error);
        }
        
        private DeviceRegistrationRepository CreateSut()
        {
            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<DeviceRegistration>(It.IsAny<DeviceRegistrationEntity>())).Returns(new DeviceRegistration("account-id", "device-token"));
            return new DeviceRegistrationRepository(new MssqlContextFactoryFake(), new Mock<IMapper>().Object);
        }
    }
}
