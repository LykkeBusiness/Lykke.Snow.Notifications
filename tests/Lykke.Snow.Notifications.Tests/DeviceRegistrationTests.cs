using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Snow.Common.Model;
using Lykke.Snow.Notifications.Domain.Enums;
using Lykke.Snow.Notifications.Domain.Model;
using Lykke.Snow.Notifications.Domain.Repositories;
using Lykke.Snow.Notifications.DomainServices.Services;
using Microsoft.Extensions.Internal;
using Moq;
using Xunit;

namespace Lykke.Snow.Notifications.Tests
{
    public class DeviceRegistrationTests
    {
        private readonly ISystemClock _systemClock = new SystemClock();

        class DeviceRegistrationTestData : IEnumerable<object[]>
        {
            public IEnumerator<object[]> GetEnumerator()
            {
                yield return new object[] { new DeviceRegistration("any-account-id", "any-device-token", DateTime.UtcNow) };
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        class DeviceRegistrationCollectionTestData : IEnumerable<object[]>
        {
            public IEnumerator<object[]> GetEnumerator()
            {
                yield return new object[] 
                { 
                    "account-id-1",
                    new List<DeviceRegistration> 
                    {
                        new DeviceRegistration("account-id-1", "device-token-1", DateTime.UtcNow), 
                        new DeviceRegistration("account-id-1", "device-token-2", DateTime.UtcNow), 
                        new DeviceRegistration("account-id-1", "device-token-3", DateTime.UtcNow)
                    }
                };
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }
        #region RegisterDevice

        [Fact]
        public async Task RegisterDevice_ShouldSet_RegisteredOn()
        {
            var mockRepository = new Mock<IDeviceRegistrationRepository>();

            var sut = CreateSut(mockRepository.Object);
            
            await sut.RegisterDeviceAsync(new DeviceRegistration("any-account-id", "any-device-token", _systemClock.UtcNow.DateTime));
            
            mockRepository.Verify(mock => mock.InsertAsync(It.Is<DeviceRegistration>(x => x.RegisteredOn != default(DateTime))));
        }

        [Theory]
        [ClassData(typeof(DeviceRegistrationTestData))]
        public async Task RegisterDevice_ShouldPassDeviceRegistration_ToInsertAsync(DeviceRegistration deviceRegistration)
        {
            var registeredOn = DateTime.UtcNow;
            var mockRepository = new Mock<IDeviceRegistrationRepository>();
            
            var systemClockMock = new Mock<ISystemClock>();
            systemClockMock.Setup(mock => mock.UtcNow).Returns(registeredOn);

            var sut = CreateSut(mockRepository.Object);
            
            await sut.RegisterDeviceAsync(deviceRegistration);
            
            mockRepository.Verify(mock => mock.InsertAsync(It.Is<DeviceRegistration>(x => 
                x.AccountId == deviceRegistration.AccountId &&
                x.DeviceToken == deviceRegistration.DeviceToken &&
                x.RegisteredOn == deviceRegistration.RegisteredOn
                )));
        }
        
        [Theory]
        [ClassData(typeof(DeviceRegistrationTestData))]
        public async Task RegisterDevice_HappyPath_ShouldNotReturnError(DeviceRegistration deviceRegistration)
        {
            var mockRepository = new Mock<IDeviceRegistrationRepository>();
            mockRepository.Setup(mock => mock.InsertAsync(deviceRegistration))
                .Returns(new Result<DeviceRegistrationErrorCode>());
            
            var sut = CreateSut(mockRepository.Object);
            
            var result = await sut.RegisterDeviceAsync(deviceRegistration);
            
            Assert.True(result.IsSuccess);
            Assert.False(result.IsFailed);
            Assert.Null(result.Error);
        }
        #endregion
        
        #region UnregisterDevice

       // [Theory]
       // [ClassData(typeof(DeviceRegistrationTestData))]
       // public async Task UnregisterDevice_HappyPath_ShouldntReturnError(DeviceRegistration deviceRegistration)
       // {
       //     var mockRepository = new Mock<IDeviceRegistrationRepository>();
       //     // Setup the mock so that it will return the given registration without any error
       //     mockRepository.Setup(mock => mock.GetDeviceRegistrationAsync(deviceRegistration.DeviceToken))
       //         .Returns(new Result<DeviceRegistration, DeviceRegistrationErrorCode>(deviceRegistration));
       //     
       //     mockRepository.Setup(mock => mock.DeleteAsync(deviceRegistration.DeviceToken))
       //         .Returns(new Result<DeviceRegistrationErrorCode>());
       //     
       //     var sut = CreateSut(mockRepository.Object);
       //     
       //     var result = await sut.UnregisterDeviceAsync(deviceRegistration);
       //     
       //     Assert.True(result.IsSuccess);
       //     Assert.False(result.IsFailed);
       //     Assert.Null(result.Error);
       // }
       // 
       // [Theory]
       // [ClassData(typeof(DeviceRegistrationTestData))]
       // public async Task UnregisterDevice_ShouldPassDeviceToken_ToDeleteAsync(DeviceRegistration deviceRegistration)
       // {
       //     var mockRepository = new Mock<IDeviceRegistrationRepository>();
       //     // Setup the mock so that it will return the given registration without any error
       //     mockRepository.Setup(x => x.GetDeviceRegistrationAsync(deviceRegistration.DeviceToken))
       //         .Returns(new Result<DeviceRegistration, DeviceRegistrationErrorCode>(deviceRegistration));
       //     
       //     var sut = CreateSut(mockRepository.Object);
       //     
       //     await sut.UnregisterDeviceAsync(deviceRegistration);

       //     mockRepository.Verify(mock => mock.DeleteAsync(It.Is<string>(x => x == deviceRegistration.DeviceToken)));
       // }
       // 
       // [Theory]
       // [ClassData(typeof(DeviceRegistrationTestData))]
       // public async Task UnregisterDevice_ShouldReturnAccountNotValid_WhenAccountIdDoesntMatch(DeviceRegistration deviceRegistration)
       // {
       //     var mockRepository = new Mock<IDeviceRegistrationRepository>();
       //     // Setup the mock so that it will return a DeviceRegistration with some different account id without any error.
       //     mockRepository.Setup(x => x.GetDeviceRegistrationAsync(deviceRegistration.DeviceToken))
       //         .Returns(new Result<DeviceRegistration, DeviceRegistrationErrorCode>(new DeviceRegistration(accountId: "some-other-account-id", deviceToken: deviceRegistration.DeviceToken)));
       //         
       //     var sut = CreateSut(mockRepository.Object);

       //     var actual = await sut.UnregisterDeviceAsync(deviceRegistration);
       //     
       //     Assert.True(actual.IsFailed);
       //     Assert.Equal(DeviceRegistrationErrorCode.AccountIdNotValid, actual.Error);
       // }
       // 

       // [Theory]
       // [ClassData(typeof(DeviceRegistrationTestData))]
       // public async Task UnregisterDevice_ShouldReturnError_IfRegistrationWasNotFound(DeviceRegistration deviceRegistration)
       // {
       //     var errorResponse = new Result<DeviceRegistration, DeviceRegistrationErrorCode>(DeviceRegistrationErrorCode.DoesNotExist);

       //     var mockRepository = new Mock<IDeviceRegistrationRepository>();
       //     // Setup the mock so that it will return an error DoesNotExist
       //     mockRepository.Setup(mock => mock.GetDeviceRegistrationAsync(deviceRegistration.DeviceToken))
       //         .ReturnsAsync(errorResponse);
       //         
       //     var sut = CreateSut(mockRepository.Object);
       //     
       //     var actual = await sut.UnregisterDeviceAsync(deviceRegistration);
       //     
       //     Assert.False(actual.IsSuccess);
       //     Assert.True(actual.IsFailed);
       //     Assert.True(actual.Error == errorResponse.Error);
       // }
       // 
       // #endregion
       // 
       // #region GetDeviceRegistrationsByAccountId

       // [Fact]
       // public async Task GetDeviceRegistrations_ShouldThrowException_IfAccountIdNotProvided()
       // {
       //     var sut = CreateSut();
       //     
       //     await Assert.ThrowsAsync<ArgumentNullException>(
       //         () => sut.GetDeviceRegistrationsAsync(""));
       // }
       // 
       // [Theory]
       // [ClassData(typeof(DeviceRegistrationCollectionTestData))]
       // public async Task GetDeviceRegistrations_HappyPath_ShouldntReturnError(string accountId, IEnumerable<DeviceRegistration> deviceRegistrations)
       // {
       //     var mockRepository = new Mock<IDeviceRegistrationRepository>();
       //     // Setup the mock so that it will return the list of DeviceRegistrations without any error
       //     mockRepository.Setup(mock => mock.GetDeviceRegistrationsByAccountIdAsync(accountId))
       //         .Returns(new Result<IEnumerable<DeviceRegistration>, DeviceRegistrationErrorCode>(deviceRegistrations));
       //     
       //     var sut = CreateSut(mockRepository.Object);
       //     
       //     var result = await sut.GetDeviceRegistrationsAsync(accountId);
       //     
       //     Assert.True(result.IsSuccess);
       //     Assert.False(result.IsFailed);
       //     Assert.Null(result.Error);
       //     Assert.Equal(deviceRegistrations, result.Value);
       // }

        #endregion
        
        
        private DeviceRegistrationService CreateSut(IDeviceRegistrationRepository? repositoryArg = null, ISystemClock? systemClockArg = null)
        {
            var repository = new Mock<IDeviceRegistrationRepository>().Object;
            ISystemClock systemClock = _systemClock;
            
            if(repositoryArg != null)
            {
                repository = repositoryArg;
            }
            if(systemClockArg != null)
            {
                systemClock = systemClockArg;
            }
            
            return new DeviceRegistrationService(repository, systemClock);
        }
    }
}
