using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Snow.Notifications.Domain.Enums;
using Lykke.Snow.Notifications.Domain.Exceptions;
using Lykke.Snow.Notifications.Domain.Model;
using Lykke.Snow.Notifications.Domain.Repositories;
using Lykke.Snow.Notifications.DomainServices.Services;
using Microsoft.Extensions.Internal;
using Moq;
using Xunit;

namespace Lykke.Snow.Notifications.Tests
{
    public class DeviceRegistrationServiceTests
    {
        private readonly ISystemClock _systemClock = new SystemClock();

        class DeviceRegistrationTestData : IEnumerable<object[]>
        {
            public IEnumerator<object[]> GetEnumerator()
            {
                yield return new object[] 
                { 
                    new DeviceRegistration("any-account-id", "any-device-token", DateTime.UtcNow) { Oid = 1 } 
                };
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
        [Theory]
        [ClassData(typeof(DeviceRegistrationTestData))]
        public async Task RegisterDevice_HappyPath_ShouldNotReturnError(DeviceRegistration deviceRegistration)
        {
            var mockRepository = new Mock<IDeviceRegistrationRepository>();
            mockRepository.Setup(mock => mock.InsertAsync(deviceRegistration))
                .Returns(Task.CompletedTask);
            
            var sut = CreateSut(mockRepository.Object);
            
            var result = await sut.RegisterDeviceAsync(deviceRegistration);
            
            Assert.True(result.IsSuccess);
            Assert.False(result.IsFailed);
            Assert.Null(result.Error);
        }

        [Theory]
        [ClassData(typeof(DeviceRegistrationTestData))]
        public async Task RegisterDevice_ShouldPassDeviceRegistration_ToInsertAsync(DeviceRegistration deviceRegistration)
        {
            var mockRepository = new Mock<IDeviceRegistrationRepository>();
            
            var sut = CreateSut(mockRepository.Object);
            
            await sut.RegisterDeviceAsync(deviceRegistration);
            
            mockRepository.Verify(mock => mock.InsertAsync(It.Is<DeviceRegistration>(x => 
                x.AccountId == deviceRegistration.AccountId &&
                x.DeviceToken == deviceRegistration.DeviceToken &&
                x.RegisteredOn == deviceRegistration.RegisteredOn
                )));
        }
        
        [Fact]
        public async Task RegisterDevice_ShouldReturnAlreadyExists_UponAlreadyExistException()
        {
            var mockRepository = new Mock<IDeviceRegistrationRepository>();
            
            // Setup the mock so that it will throw EntityAlreadyExistsException
            mockRepository.Setup(mock => mock.InsertAsync(It.IsAny<DeviceRegistration>()))
                .Throws<EntityAlreadyExistsException>();
            
            var sut = CreateSut(mockRepository.Object);
            
            var actual = await sut.RegisterDeviceAsync(It.IsAny<DeviceRegistration>());

            Assert.True(actual.IsFailed);
            Assert.False(actual.IsSuccess);
            Assert.Equal(DeviceRegistrationErrorCode.AlreadyRegistered, actual.Error);
        }
        #endregion
        
        #region UnregisterDevice
        [Theory]
        [ClassData(typeof(DeviceRegistrationTestData))]
        public async Task UnregisterDevice_ShouldReturnDoesNotExistError_IfRegistrationWasNotFound(DeviceRegistration deviceRegistration)
        {
            var mockRepository = new Mock<IDeviceRegistrationRepository>();

            // Setup the mock so that it will return null
            mockRepository.Setup(mock => mock.GetDeviceRegistrationAsync(deviceRegistration.DeviceToken))
                .Returns(Task.FromResult<DeviceRegistration>(null));
                
            var sut = CreateSut(mockRepository.Object);
            
            var actual = await sut.UnregisterDeviceAsync(deviceToken: deviceRegistration.DeviceToken, accountId: deviceRegistration.AccountId);
            
            Assert.False(actual.IsSuccess);
            Assert.True(actual.IsFailed);
            Assert.True(actual.Error == DeviceRegistrationErrorCode.DoesNotExist);
        }

        [Theory]
        [ClassData(typeof(DeviceRegistrationTestData))]
        public async Task UnregisterDevice_ShouldReturnDoesNotExistError_UponEntityNotFoundException(DeviceRegistration deviceRegistration)
        {
            var mockRepository = new Mock<IDeviceRegistrationRepository>();

            // Setup the mock so that it will throw EntityNotFoundException
            mockRepository.Setup(mock => mock.GetDeviceRegistrationAsync(deviceRegistration.DeviceToken))
                .Throws(new EntityNotFoundException());
                
            var sut = CreateSut(mockRepository.Object);
            
            var actual = await sut.UnregisterDeviceAsync(deviceToken: deviceRegistration.DeviceToken, accountId: deviceRegistration.AccountId);
            
            Assert.False(actual.IsSuccess);
            Assert.True(actual.IsFailed);
            Assert.True(actual.Error == DeviceRegistrationErrorCode.DoesNotExist);
        }

        [Theory]
        [ClassData(typeof(DeviceRegistrationTestData))]
        public async Task UnregisterDevice_HappyPath_ShouldntReturnError(DeviceRegistration deviceRegistration)
        {
            var mockRepository = new Mock<IDeviceRegistrationRepository>();

            // Setup the mock so that GetDeviceRegistrationAsync() will return the given registration without any error
            mockRepository.Setup(mock => mock.GetDeviceRegistrationAsync(deviceRegistration.DeviceToken))
                .ReturnsAsync(deviceRegistration);
            
            // Setup the mock so that DeleteAsync() gets executed without exception
            mockRepository.Setup(mock => mock.DeleteAsync(deviceRegistration.Oid))
                .Returns(Task.CompletedTask);
            
            var sut = CreateSut(mockRepository.Object);
            
            var result = await sut.UnregisterDeviceAsync(deviceToken: deviceRegistration.DeviceToken, accountId: deviceRegistration.AccountId);
            
            Assert.True(result.IsSuccess);
            Assert.False(result.IsFailed);
            Assert.Null(result.Error);
        }
        
        [Theory]
        [ClassData(typeof(DeviceRegistrationTestData))]
        public async Task UnregisterDevice_ShouldPassOid_ToDeleteAsync(DeviceRegistration deviceRegistration)
        {
            var mockRepository = new Mock<IDeviceRegistrationRepository>();

            // Setup the mock so that it will return the given registration without any error
            mockRepository.Setup(x => x.GetDeviceRegistrationAsync(deviceRegistration.DeviceToken))
                .ReturnsAsync(deviceRegistration);
            
            var sut = CreateSut(mockRepository.Object);
            
            await sut.UnregisterDeviceAsync(deviceToken: deviceRegistration.DeviceToken, accountId: deviceRegistration.AccountId);

            mockRepository.Verify(mock => 
                mock.DeleteAsync(It.Is<int>(x => x == deviceRegistration.Oid)));
        }
        
        [Theory]
        [ClassData(typeof(DeviceRegistrationTestData))]
        public async Task UnregisterDevice_ShouldReturnAccountNotValid_WhenAccountIdDoesntMatch(DeviceRegistration deviceRegistration)
        {
            var mockRepository = new Mock<IDeviceRegistrationRepository>();

            // Setup the mock so that it will return a DeviceRegistration with some different account id without any error.
            mockRepository.Setup(x => x.GetDeviceRegistrationAsync(deviceRegistration.DeviceToken))
                .ReturnsAsync(new DeviceRegistration(accountId: "some-other-account-id", deviceToken: deviceRegistration.DeviceToken, _systemClock.UtcNow.DateTime));
                
            var sut = CreateSut(mockRepository.Object);

            var actual = await sut.UnregisterDeviceAsync(deviceToken: deviceRegistration.DeviceToken, accountId: deviceRegistration.AccountId);
            
            Assert.True(actual.IsFailed);
            Assert.False(actual.IsSuccess);
            Assert.Equal(DeviceRegistrationErrorCode.AccountIdNotValid, actual.Error);
        }
        
       [Theory]
       [ClassData(typeof(DeviceRegistrationTestData))]
       public async Task UnregisterDevice_ShouldReturnDoesNotExist_UponEntityNotFoundException(DeviceRegistration deviceRegistration)
       {
            var mockRepository = new Mock<IDeviceRegistrationRepository>();

            // Setup the mock so that GetDeviceRegistrationAsync() will return the given registration without any error
            mockRepository.Setup(mock => mock.GetDeviceRegistrationAsync(deviceRegistration.DeviceToken))
                .ReturnsAsync(deviceRegistration);
            
            // Setup the mock so that it will throw EntityNotFoundException
            mockRepository.Setup(mock => mock.DeleteAsync(deviceRegistration.Oid))
                .Throws<EntityNotFoundException>();
            
            var sut = CreateSut(mockRepository.Object);
            
            var actual = await sut.UnregisterDeviceAsync(deviceToken: deviceRegistration.DeviceToken, accountId: deviceRegistration.AccountId);

            Assert.True(actual.IsFailed);
            Assert.False(actual.IsSuccess);
            Assert.Equal(DeviceRegistrationErrorCode.DoesNotExist, actual.Error);
       }
       #endregion

        
       #region GetDeviceRegistrationsByAccountId

       [Fact]
       public async Task GetDeviceRegistrations_ShouldThrowException_IfAccountIdNotProvided()
       {
           var sut = CreateSut();
           
           await Assert.ThrowsAsync<ArgumentNullException>(
               () => sut.GetDeviceRegistrationsAsync(""));
       }

       
       [Theory]
       [ClassData(typeof(DeviceRegistrationCollectionTestData))]
       public async Task GetDeviceRegistrations_HappyPath_ShouldntReturnError(string accountId, IReadOnlyList<DeviceRegistration> deviceRegistrations)
       {
           var mockRepository = new Mock<IDeviceRegistrationRepository>();

           // Setup the mock so that it will return the list of DeviceRegistrations without any error
           mockRepository.Setup(mock => mock.GetDeviceRegistrationsByAccountIdAsync(accountId))
               .ReturnsAsync(deviceRegistrations);
           
           var sut = CreateSut(mockRepository.Object);
           
           var result = await sut.GetDeviceRegistrationsAsync(accountId);
           
           Assert.True(result.IsSuccess);
           Assert.False(result.IsFailed);
           Assert.Null(result.Error);
           Assert.Equal(deviceRegistrations, result.Value);
       }
       
       [Fact]
       public async Task GetDeviceRegistrations_ShouldReturnEmptyCollection_IfRepositoryReturnsNull()
       {
           var mockRepository = new Mock<IDeviceRegistrationRepository>();

           // Setup the mock so that it will return the list of DeviceRegistrations without any error
           mockRepository.Setup(mock => mock.GetDeviceRegistrationsByAccountIdAsync(It.IsAny<string>()))
               .Returns(Task.FromResult<IReadOnlyList<DeviceRegistration>>(null));
            
            var sut = CreateSut(mockRepository.Object);
            
            var actual = await sut.GetDeviceRegistrationsAsync("any-account-id");
            
            Assert.True(actual.IsSuccess);
            Assert.False(actual.IsFailed);
            Assert.Empty(actual.Value);
       }

       #endregion
        
        
        private DeviceRegistrationService CreateSut(IDeviceRegistrationRepository repositoryArg = null, ISystemClock systemClockArg = null)
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
