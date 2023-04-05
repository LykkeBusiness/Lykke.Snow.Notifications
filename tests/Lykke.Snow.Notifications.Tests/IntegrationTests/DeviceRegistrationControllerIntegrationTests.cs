using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Lykke.Snow.Notifications.Client.Model;
using Lykke.Snow.Notifications.Client.Model.Requests;
using Lykke.Snow.Notifications.Client.Models;
using Lykke.Snow.Notifications.Domain.Enums;
using Lykke.Snow.Notifications.Domain.Repositories;
using Lykke.Snow.Notifications.Domain.Services;
using Lykke.Snow.Notifications.Tests.Extensions;
using Lykke.Snow.Notifications.Tests.Fakes;
using Xunit;

namespace Lykke.Snow.Notifications.Tests.IntegrationTests
{
    public class DeviceRegistrationControllerIntegrationTests : IClassFixture<WebAppFactoryNoDependencies>
    {
        private readonly HttpClient _client;

        private readonly IServiceProvider _serviceProvider;

        public DeviceRegistrationControllerIntegrationTests(WebAppFactoryNoDependencies factory)
        {
            _client = factory.CreateSecuredClient();
            _serviceProvider = factory.Services;
        }

        #region POST /api/DeviceRegistration

        [Fact]
        public async Task RegisterDevice_ShouldSucceed_WithValidArguments()
        {
            FcmIntegrationServiceFake.SetIsDeviceTokenValid(true);

            var registerDeviceRequest = new RegisterDeviceRequest(accountId: Guid.NewGuid().ToString(), deviceToken: Guid.NewGuid().ToString(), deviceId: Guid.NewGuid().ToString(), "en");
            
            var response = await _client.PostAsJsonAsync("/api/DeviceRegistration", registerDeviceRequest);

            await response.AssertSuccessAsync(DeviceRegistrationErrorCode.None);
        }

        [Fact]
        public async Task RegisterDevice_ShouldReturnDeviceTokenNotValid_ForInvalidDeviceToken()
        {
            FcmIntegrationServiceFake.SetIsDeviceTokenValid(false);

            var registerDeviceRequest = new RegisterDeviceRequest(accountId: Guid.NewGuid().ToString(), deviceToken: Guid.NewGuid().ToString(), deviceId: Guid.NewGuid().ToString(), "en");

            var response = await _client.PostAsJsonAsync("/api/DeviceRegistration", registerDeviceRequest);

            await response.AssertErrorAsync(DeviceRegistrationErrorCode.DeviceTokenNotValid);
        }

        [Fact]
        public async Task RegisterDevice_InvalidLocale_ShouldReturnErrorCode()
        {
            FcmIntegrationServiceFake.SetIsDeviceTokenValid(true);

            var registerDeviceRequest = new RegisterDeviceRequest(accountId: Guid.NewGuid().ToString(), deviceToken: Guid.NewGuid().ToString(), 
                    deviceId: Guid.NewGuid().ToString(), "lang-invalid");
            
            var response = await _client.PostAsJsonAsync("/api/DeviceRegistration", registerDeviceRequest);

            await response.AssertErrorAsync(DeviceConfigurationErrorCodeContract.UnsupportedLocale);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public async Task RegisterDevice_InvalidAccountId_ShouldReturnInvalidInput(string accountId)
        {
            FcmIntegrationServiceFake.SetIsDeviceTokenValid(true);

            var registerDeviceRequest = new RegisterDeviceRequest(accountId: accountId, deviceToken: Guid.NewGuid().ToString(), 
                    deviceId: Guid.NewGuid().ToString(), "lang-invalid");
            
            var response = await _client.PostAsJsonAsync("/api/DeviceRegistration", registerDeviceRequest);

            await response.AssertErrorAsync(DeviceRegistrationErrorCodeContract.InvalidInput);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public async Task RegisterDevice_InvalidDeviceId_ShouldReturnInvalidInput(string deviceId)
        {
            FcmIntegrationServiceFake.SetIsDeviceTokenValid(true);

            var registerDeviceRequest = new RegisterDeviceRequest("account-id", "device-token", deviceId, "en");
            
            var response = await _client.PostAsJsonAsync("/api/DeviceRegistration", registerDeviceRequest);

            await response.AssertErrorAsync(DeviceRegistrationErrorCodeContract.InvalidInput);
        }

        [Fact]
        public async Task RegisterDevice_WithValidPayload_ShouldCreateTheRegistration()
        {
            FcmIntegrationServiceFake.SetIsDeviceTokenValid(true);

            var guid = Guid.NewGuid().ToString();
            var registerDeviceRequest = new RegisterDeviceRequest(guid, "device-token", "device-id", "en");

            var response = await _client.PostAsJsonAsync("/api/DeviceRegistration", registerDeviceRequest);
            
            await response.AssertSuccessAsync(DeviceRegistrationErrorCode.None);

            IDeviceRegistrationService? deviceRegistrationService = 
                _serviceProvider.GetService(typeof(IDeviceRegistrationService)) as IDeviceRegistrationService;
                
            if(deviceRegistrationService == null)
                throw new NullReferenceException();
            
            var deviceRegistrationResult = await deviceRegistrationService.GetDeviceRegistrationsAsync(guid);

            var registeredDevice = deviceRegistrationResult.Value.First();
            
            Assert.Equal(expected: registerDeviceRequest.AccountId, registeredDevice.AccountId);
            Assert.Equal(expected: registerDeviceRequest.DeviceToken, registeredDevice.DeviceToken);
            Assert.Equal(expected: registerDeviceRequest.DeviceId, registeredDevice.DeviceId);
        }

        [Fact]
        public async Task RegisterDevice_WithMultipleAccounts_ShouldCreateMutipleNotificationConfiguration()
        {
            FcmIntegrationServiceFake.SetIsDeviceTokenValid(true);

            const string deviceId = "device-id";
            const string lang = "en";
            const string deviceToken = "device-token";

            var firstAccountId = Guid.NewGuid().ToString();
            var registerDeviceRequest = new RegisterDeviceRequest(firstAccountId, deviceToken, deviceId, lang);

            var response = await _client.PostAsJsonAsync("/api/DeviceRegistration", registerDeviceRequest);
            await response.AssertSuccessAsync(DeviceRegistrationErrorCode.None);
            
            var newAccountId = Guid.NewGuid().ToString();
            registerDeviceRequest = new RegisterDeviceRequest(newAccountId, deviceToken, deviceId, lang);

            response = await _client.PostAsJsonAsync("/api/DeviceRegistration", registerDeviceRequest);
            await response.AssertSuccessAsync(DeviceRegistrationErrorCode.None);

            IDeviceConfigurationRepository? deviceConfigurationRepository = 
                _serviceProvider.GetService(typeof(IDeviceConfigurationRepository)) as IDeviceConfigurationRepository;
                
            if(deviceConfigurationRepository == null)
                throw new NullReferenceException();
            
            var firstConfiguration = await deviceConfigurationRepository.GetAsync(deviceId, firstAccountId);
            var secondConfiguration = await deviceConfigurationRepository.GetAsync(deviceId, newAccountId);
            
            Assert.NotEqual(firstConfiguration.AccountId, secondConfiguration.AccountId);
            Assert.Equal(firstConfiguration.DeviceId, secondConfiguration.DeviceId);
        }


        #endregion
        
        #region DELETE /api/DeviceRegistration

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public async Task UnregisterDevice_WithNullOrEmptyDeviceToken_ShouldReturnInvalidInput(string deviceToken)
        {
            var unregisterDeviceRequest = new UnregisterDeviceRequest(deviceToken);
            
            var response = await _client.DeleteWithPayloadAsync("/api/DeviceRegistration", unregisterDeviceRequest);

            await response.AssertErrorAsync(DeviceRegistrationErrorCodeContract.InvalidInput);
        }

        [Fact]
        public async Task UnregisterDevice_WithDeviceTokenThatDoesntExist_ShouldReturnNone()
        {
            var unregisterDeviceRequest = new UnregisterDeviceRequest("device-token-that-does-not-exist");
            
            var response = await _client.DeleteWithPayloadAsync("/api/DeviceRegistration", unregisterDeviceRequest);
            
            await response.AssertErrorAsync(DeviceRegistrationErrorCode.None);
        }

        [Fact]
        public async Task UnregisterDevice_WithValidDeviceToken_ShouldRemoveDeviceToken()
        {
            FcmIntegrationServiceFake.SetIsDeviceTokenValid(true);
            
            // Register new device
            var accountId = Guid.NewGuid().ToString();
            var deviceToken = Guid.NewGuid().ToString();
            var registerDeviceRequest = new RegisterDeviceRequest(accountId, deviceToken, "device-id", "en");

            IDeviceRegistrationService? deviceRegistrationService = 
                _serviceProvider.GetService(typeof(IDeviceRegistrationService)) as IDeviceRegistrationService;
                
            if(deviceRegistrationService == null)
                throw new NullReferenceException();

            var response = await _client.PostAsJsonAsync("/api/DeviceRegistration", registerDeviceRequest);

            // Make sure it's been created
            await response.AssertSuccessAsync(DeviceRegistrationErrorCode.None);
            var deviceRegistrationResult = await deviceRegistrationService.GetDeviceRegistrationsAsync(accountId);
            var registeredDevice = deviceRegistrationResult.Value.First();

            Assert.Equal(accountId, registeredDevice.AccountId);
            Assert.Equal(deviceToken, registeredDevice.DeviceToken);
                
            // Unregister it
            var unregisterDeviceRequest = new UnregisterDeviceRequest(deviceToken);
            response = await _client.DeleteWithPayloadAsync("/api/DeviceRegistration", unregisterDeviceRequest);
            await response.AssertSuccessAsync(DeviceRegistrationErrorCode.None);

            deviceRegistrationResult = await deviceRegistrationService.GetDeviceRegistrationsAsync(deviceToken);
            
            // Make sure it doesn't exist anymore
            Assert.Empty(deviceRegistrationResult.Value);
        }
        #endregion
        
    }
}
