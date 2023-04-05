using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using FsCheck;
using FsCheck.Xunit;
using Lykke.Snow.FirebaseIntegration.Interfaces;
using Lykke.Snow.Notifications.Domain.Enums;
using Lykke.Snow.Notifications.Domain.Exceptions;
using Lykke.Snow.Notifications.Domain.Model;
using Lykke.Snow.Notifications.Domain.Repositories;
using Lykke.Snow.Notifications.DomainServices.Services;
using Moq;
using Xunit;

namespace Lykke.Snow.Notifications.Tests
{
    public class DeviceRegistrationServiceTests
    {
        class DeviceRegistrationTestData : IEnumerable<object[]>
        {
            public IEnumerator<object[]> GetEnumerator()
            {
                yield return new object[] 
                { 
                    new DeviceRegistration("any-account-id", "any-device-token", "any-device-id", DateTime.UtcNow) { Oid = 1 } 
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
                        new DeviceRegistration("account-id-1", "device-token-1", "device-id-1", DateTime.UtcNow), 
                        new DeviceRegistration("account-id-1", "device-token-2", "device-id-2", DateTime.UtcNow), 
                        new DeviceRegistration("account-id-1", "device-token-3", "device-id-3", DateTime.UtcNow)
                    }
                };
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        class DeviceRegistrationCollectionTestDataSameDeviceToken : IEnumerable<object[]>
        {
            public IEnumerator<object[]> GetEnumerator()
            {
                yield return new object[] 
                { 
                    new List<DeviceRegistration> 
                    {
                        new DeviceRegistration("account-id-1", "device-token-1", "device-id-1", DateTime.UtcNow) { Oid = 1 }, 
                        new DeviceRegistration("account-id-2", "device-token-1", "device-id-1", DateTime.UtcNow) { Oid = 2 }, 
                        new DeviceRegistration("account-id-3", "device-token-1", "device-id-1", DateTime.UtcNow) { Oid = 3 }
                    }
                };
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        class MultipleAccountIdsDeviceRegistrationCollectionTestData : IEnumerable<object[]>
        {
            public IEnumerator<object[]> GetEnumerator()
            {
                yield return new object[] 
                {
                    new[] { "account-id-1", "account-id-2", "account-id-3" },
                    new List<DeviceRegistration> 
                    {
                        new DeviceRegistration("account-id-1", "device-token-1", "device-id-1", DateTime.UtcNow), 
                        new DeviceRegistration("account-id-2", "device-token-2", "device-id-2", DateTime.UtcNow), 
                        new DeviceRegistration("account-id-3", "device-token-3", "device-id-3", DateTime.UtcNow)
                    }
                };
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }
        
        class UnsupportedLocaleCollectionTestData : IEnumerable<object[]>
        {
            public IEnumerator<object[]> GetEnumerator()
            {
                yield return new object[] { "" };
                yield return new object[] { " " };
                yield return new object[] { null };
                yield return new object[] { "unsupported-locale" };
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }
        

        #region RegisterDevice
        
        [Property]
        public Property RegisterDevice_HappyPath_ShouldNotReturnError()
        {
            return Prop.ForAll(Gens.DeviceRegistration.ToArbitrary(), dr =>
            {
                var mockRepository = new Mock<IDeviceRegistrationRepository>();
                mockRepository.Setup(mock => mock.AddOrUpdateAsync(dr))
                    .Returns(Task.CompletedTask);

                var mockFcmIntegrationService = new Mock<IFcmIntegrationService>();
                mockFcmIntegrationService.Setup(mock => mock.IsDeviceTokenValid(dr.DeviceToken)).ReturnsAsync(true);

                var sut = CreateSut(mockRepository.Object, mockFcmIntegrationService.Object);

                var result = sut.RegisterDeviceAsync(dr, "en").Result;

                return result.IsSuccess && !result.IsFailed && result.Error == null;
            });
        }

        [Theory]
        [ClassData(typeof(DeviceRegistrationTestData))]
        public async Task RegisterDevice_ShouldPassDeviceRegistration_ToInsertAsync(DeviceRegistration deviceRegistration)
        {
            var mockRepository = new Mock<IDeviceRegistrationRepository>();
            
            var mockFcmIntegrationService = new Mock<IFcmIntegrationService>();
            mockFcmIntegrationService.Setup(mock => mock.IsDeviceTokenValid(deviceRegistration.DeviceToken)).ReturnsAsync(true);

            var sut = CreateSut(mockRepository.Object, mockFcmIntegrationService.Object);
            
            await sut.RegisterDeviceAsync(deviceRegistration, "en");
            
            mockRepository.Verify(mock => mock.AddOrUpdateAsync(It.Is<DeviceRegistration>(x => 
                x.AccountId == deviceRegistration.AccountId &&
                x.DeviceToken == deviceRegistration.DeviceToken &&
                x.RegisteredOn == deviceRegistration.RegisteredOn
                )), Times.Once);
        }
      
        [Theory]
        [ClassData(typeof(DeviceRegistrationTestData))]
        public async Task RegisterDevice_ShouldReturnAlreadyExists_UponAlreadyExistException(DeviceRegistration deviceRegistration)
        {
            var mockRepository = new Mock<IDeviceRegistrationRepository>();
            
            // Setup the mock so that it will throw EntityAlreadyExistsException
            mockRepository.Setup(mock => mock.AddOrUpdateAsync(deviceRegistration))
                .Throws<EntityAlreadyExistsException>();

            var mockFcmIntegrationService = new Mock<IFcmIntegrationService>();
            mockFcmIntegrationService.Setup(mock => mock.IsDeviceTokenValid(deviceRegistration.DeviceToken))
                .ReturnsAsync(true);
            
            var sut = CreateSut(mockRepository.Object, mockFcmIntegrationService.Object);
            
            var actual = await sut.RegisterDeviceAsync(deviceRegistration, "en");

            Assert.True(actual.IsFailed);
            Assert.False(actual.IsSuccess);
            Assert.Equal(DeviceRegistrationErrorCode.AlreadyRegistered, actual.Error);
        }

        [Theory]
        [ClassData(typeof(UnsupportedLocaleCollectionTestData))]
        public async Task RegisterDevice_ShouldReturnUnsupportedLocale_UponUnsupportedLocaleException(
            string unsupportedLocale)
        {
            var mockFcmIntegrationService = new Mock<IFcmIntegrationService>();
            mockFcmIntegrationService.Setup(mock => mock.IsDeviceTokenValid(It.IsAny<string>())).ReturnsAsync(true);

            var sut = CreateSut(Mock.Of<IDeviceRegistrationRepository>(), mockFcmIntegrationService.Object);
            
            var actual = await sut.RegisterDeviceAsync(
                new DeviceRegistration("account-id", "device-token", "device-id", DateTime.UtcNow), unsupportedLocale);
            
            Assert.True(actual.IsFailed);
            Assert.Equal(DeviceRegistrationErrorCode.UnsupportedLocale, actual.Error);
        }

        [Theory]
        [ClassData(typeof(DeviceRegistrationTestData))]
        public async Task RegisterDevice_ShouldReturnInvalidFcmToken_IfDeviceTokenIsInvalid(DeviceRegistration deviceRegistration)
        {
            var mockFcmIntegrationService = new Mock<IFcmIntegrationService>();
            mockFcmIntegrationService.Setup(mock => mock.IsDeviceTokenValid(deviceRegistration.DeviceToken))
                .ReturnsAsync(false);
            
            var sut = CreateSut(fcmIntegrationServiceArg: mockFcmIntegrationService.Object);
            
            var actual = await sut.RegisterDeviceAsync(deviceRegistration, "any-locale");

            Assert.True(actual.IsFailed);
            Assert.False(actual.IsSuccess);
            Assert.Equal(DeviceRegistrationErrorCode.DeviceTokenNotValid, actual.Error);
        }

        [Theory]
        [ClassData(typeof(DeviceRegistrationTestData))]
        public async Task RegisterDevice_ShouldAdd_DeviceConfiguration(DeviceRegistration deviceRegistration)
        {
            var mockFcmIntegrationService = new Mock<IFcmIntegrationService>();
            mockFcmIntegrationService.Setup(mock => mock.IsDeviceTokenValid(deviceRegistration.DeviceToken))
                .ReturnsAsync(true);
                
            var mockDeviceConfigurationRepository = new Mock<IDeviceConfigurationRepository>();
            
            var sut = CreateSut(fcmIntegrationServiceArg: mockFcmIntegrationService.Object, 
                deviceConfigurationRepositoryArg: mockDeviceConfigurationRepository.Object);
            
            var deviceId = "any-device-id";
            var locale = Locale.En;

            await sut.RegisterDeviceAsync(deviceRegistration, locale.ToString());
            
            mockDeviceConfigurationRepository.Verify(x => x.AddOrUpdateAsync(It.Is<DeviceConfiguration>(
                dc => 
                dc.AccountId == deviceRegistration.AccountId &&
                dc.DeviceId == deviceId &&
                dc.Locale == locale
            )), Times.Once);
        }
        
        [Theory]
        [InlineData("device-id-1", "account-id-1")]
        public void RegisterDevice_ShouldntAddDeviceConfiguration_IfThereIsAlreadyOne(string deviceId, string accountId)
        {
            var mockFcmIntegrationService = new Mock<IFcmIntegrationService>();
            mockFcmIntegrationService.Setup(mock => mock.IsDeviceTokenValid(It.IsAny<string>()))
                .ReturnsAsync(true);
                
            var mockDeviceConfigurationRepository = new Mock<IDeviceConfigurationRepository>();
            mockDeviceConfigurationRepository.Setup(mock => mock.GetAsync(deviceId, accountId))
                .ReturnsAsync(new DeviceConfiguration(deviceId, accountId, "en"));
            
            var sut = CreateSut(fcmIntegrationServiceArg: mockFcmIntegrationService.Object, 
                deviceConfigurationRepositoryArg: mockDeviceConfigurationRepository.Object);
        
            mockDeviceConfigurationRepository.Verify(x => x.AddOrUpdateAsync(It.IsAny<DeviceConfiguration>()), Times.Never);
        }

        #endregion

        #region UnregisterDevice
        [Theory]
        [ClassData(typeof(DeviceRegistrationTestData))]
        public async Task UnregisterDevice_ShouldNotCallRemoveAsync_IfNoRegistrationFound(DeviceRegistration deviceRegistration)
        {
            var mockRepository = new Mock<IDeviceRegistrationRepository>();

            // Setup the mock so that it will return null
            mockRepository.Setup(mock => mock.GetDeviceRegistrationsAsync(deviceRegistration.DeviceToken))
                .Returns(Task.FromResult(new List<DeviceRegistration>() as IReadOnlyList<DeviceRegistration>));
                
            var sut = CreateSut(mockRepository.Object);
            
            var actual = await sut.UnregisterDeviceAsync(deviceToken: deviceRegistration.DeviceToken);
            
            Assert.Null(actual.Error);

            mockRepository.Verify(x => x.RemoveAllAsync(It.IsAny<int[]>()), Times.Never);
        }

      [Theory]
      [ClassData(typeof(DeviceRegistrationTestData))]
      public async Task UnregisterDevice_ShouldReturnDoesNotExistError_UponEntityNotFoundException(DeviceRegistration deviceRegistration)
      {
          var mockRepository = new Mock<IDeviceRegistrationRepository>();

          // Setup the mock so that it will throw EntityNotFoundException
          mockRepository.Setup(mock => mock.GetDeviceRegistrationsAsync(deviceRegistration.DeviceToken))
              .Throws(new EntityNotFoundException());
              
          var sut = CreateSut(mockRepository.Object);
          
          var actual = await sut.UnregisterDeviceAsync(deviceToken: deviceRegistration.DeviceToken);
          
          Assert.False(actual.IsSuccess);
          Assert.True(actual.IsFailed);
          Assert.True(actual.Error == DeviceRegistrationErrorCode.DoesNotExist);
      }

      [Theory]
      [ClassData(typeof(DeviceRegistrationCollectionTestDataSameDeviceToken))]
      public async Task UnregisterDevice_HappyPath_ShouldntReturnError(List<DeviceRegistration> deviceRegistrations)
      {
          var mockRepository = new Mock<IDeviceRegistrationRepository>();

          // Setup the mock so that GetDeviceRegistrationAsync() will return the given registration without any error
          mockRepository.Setup(mock => mock.GetDeviceRegistrationsAsync(It.IsAny<string>()))
              .ReturnsAsync(deviceRegistrations);
          
          // Setup the mock so that RemoveAsync() gets executed without exception
          mockRepository.Setup(mock => mock.RemoveAllAsync(It.IsAny<int[]>()))
              .Returns(Task.CompletedTask);
          
          var sut = CreateSut(mockRepository.Object);
          
          var result = await sut.UnregisterDeviceAsync(deviceToken: deviceRegistrations[0].DeviceToken);

          Assert.True(result.IsSuccess);
          Assert.False(result.IsFailed);
          Assert.Null(result.Error);
      }

       
      [Theory]
      [ClassData(typeof(DeviceRegistrationCollectionTestDataSameDeviceToken))]
      public async Task UnregisterDevice_Verify_RemoveAsyncCalls(List<DeviceRegistration> deviceRegistrations)
      {
          var mockRepository = new Mock<IDeviceRegistrationRepository>();

          // Setup the mock so that it will return the given registration without any error
          mockRepository.Setup(x => x.GetDeviceRegistrationsAsync(deviceRegistrations[0].DeviceToken))
              .ReturnsAsync(deviceRegistrations);
          
          var sut = CreateSut(mockRepository.Object);
          
          await sut.UnregisterDeviceAsync(deviceToken: deviceRegistrations[0].DeviceToken);

         mockRepository.Verify(mock => 
                  mock.RemoveAllAsync(It.IsAny<int[]>()), Times.Once);
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

       #region GetDeviceregistrationsByMultipleAccountIds

       [Fact]
       public async Task GetDeviceRegistrationsByAccountIds_ShouldThrowException_IfNullArrayIsNotProvided()
       {
           var sut = CreateSut();
           
           await Assert.ThrowsAsync<ArgumentNullException>(
               () => sut.GetDeviceRegistrationsAsync((string[]?)null));
       }

       [Theory]
       [ClassData(typeof(MultipleAccountIdsDeviceRegistrationCollectionTestData))]
       public async Task GetDeviceRegistrationsByAccountIds_HappyPath_ShouldntReturnError(string[] accountIds, IReadOnlyList<DeviceRegistration> deviceRegistrations)
       {
           var mockRepository = new Mock<IDeviceRegistrationRepository>();

           // Setup the mock so that it will return the list of DeviceRegistrations without any error
           mockRepository.Setup(mock => mock.GetDeviceRegistrationsByAccountIdsAsync(accountIds))
               .ReturnsAsync(deviceRegistrations);
           
           var sut = CreateSut(mockRepository.Object);
           
           var result = await sut.GetDeviceRegistrationsAsync(accountIds);
           
           Assert.True(result.IsSuccess);
           Assert.False(result.IsFailed);
           Assert.Null(result.Error);
           Assert.Equal(deviceRegistrations, result.Value);
       }

       [Fact]
       public async Task GetDeviceRegistrationsByMultipleAccountIds_ShouldReturnEmptyCollection_IfRepositoryReturnsNull()
       {
           var mockRepository = new Mock<IDeviceRegistrationRepository>();

           // Setup the mock so that it will return the list of DeviceRegistrations without any error
           mockRepository.Setup(mock => mock.GetDeviceRegistrationsByAccountIdsAsync(It.IsAny<string[]>()))
               .Returns(Task.FromResult<IReadOnlyList<DeviceRegistration>>(null));
            
            var sut = CreateSut(mockRepository.Object);
            
            var actual = await sut.GetDeviceRegistrationsAsync(new[] { "account-id-1", "account-id-2", "account-id-3" });
            
            Assert.True(actual.IsSuccess);
            Assert.False(actual.IsFailed);
            Assert.Empty(actual.Value);
       }

       #endregion
        
        
        private DeviceRegistrationService CreateSut(IDeviceRegistrationRepository repositoryArg = null, 
            IFcmIntegrationService fcmIntegrationServiceArg = null, 
            IDeviceConfigurationRepository deviceConfigurationRepositoryArg = null)
        {
            var deviceRegistrationRepository = new Mock<IDeviceRegistrationRepository>().Object;
            var deviceConfigurationRepository = new Mock<IDeviceConfigurationRepository>().Object;

            IFcmIntegrationService fcmIntegrationService = new Mock<IFcmIntegrationService>().Object;
            
            if(repositoryArg != null)
            {
                deviceRegistrationRepository = repositoryArg;
            }
            if(deviceConfigurationRepositoryArg != null)
            {
                deviceConfigurationRepository = deviceConfigurationRepositoryArg;
            }
            if(fcmIntegrationServiceArg != null)
            {
                fcmIntegrationService = fcmIntegrationServiceArg;
            }
            
            return new DeviceRegistrationService(deviceRegistrationRepository, fcmIntegrationService, deviceConfigurationRepository);
        }
    }
}
