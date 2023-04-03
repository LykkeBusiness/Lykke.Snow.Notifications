using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Lykke.Snow.Notifications.Client.Model.Requests;
using Lykke.Snow.Notifications.Client.Models;
using Lykke.Snow.Notifications.Domain.Enums;
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

            var registerDeviceRequest = new RegisterDeviceRequest("account-id", "device-token", "device-id", "en");
            
            var response = await _client.PostAsJsonAsync("/api/DeviceRegistration", registerDeviceRequest);

            await response.AssertSuccessAsync(DeviceRegistrationErrorCode.None);
        }

        [Fact]
        public async Task RegisterDevice_ShouldReturnDeviceTokenNotValid_ForInvalidDeviceToken()
        {
            FcmIntegrationServiceFake.SetIsDeviceTokenValid(false);

            var registerDeviceRequest = new RegisterDeviceRequest("account-id", "invalid-device-token", "device-id", "en");

            var response = await _client.PostAsJsonAsync("/api/DeviceRegistration", registerDeviceRequest);

            await response.AssertErrorAsync(DeviceRegistrationErrorCode.DeviceTokenNotValid);
        }

        [Fact]
        public async Task RegisterDevice_InvalidLocale_ShouldReturnErrorCode()
        {
            FcmIntegrationServiceFake.SetIsDeviceTokenValid(true);

            var registerDeviceRequest = new RegisterDeviceRequest("account-id", "device-token", "device-id", "lang-invalid");
            
            var response = await _client.PostAsJsonAsync("/api/DeviceRegistration", registerDeviceRequest);

            await response.AssertErrorAsync(DeviceConfigurationErrorCodeContract.UnsupportedLocale);
        }

        [Fact]
        public async Task RegisterDevice_InvalidAccountId_ShouldReturnBadRequest()
        {
            FcmIntegrationServiceFake.SetIsDeviceTokenValid(true);

            var registerDeviceRequest = new RegisterDeviceRequest("", "device-token", "device-id", "lang-invalid");
            
            var response = await _client.PostAsJsonAsync("/api/DeviceRegistration", registerDeviceRequest);

            response.AssertHttpStatusCode(HttpStatusCode.BadRequest);

            registerDeviceRequest = new RegisterDeviceRequest(null, "device-token", "device-id", "en");

            response = await _client.PostAsJsonAsync("/api/DeviceRegistration", registerDeviceRequest);

            response.AssertHttpStatusCode(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task RegisterDevice_InvalidDeviceId_ShouldReturnBadRequest()
        {
            FcmIntegrationServiceFake.SetIsDeviceTokenValid(true);

            var registerDeviceRequest = new RegisterDeviceRequest("account-id", "device-token", "", "en");
            
            var response = await _client.PostAsJsonAsync("/api/DeviceRegistration", registerDeviceRequest);

            response.AssertHttpStatusCode(HttpStatusCode.BadRequest);

            registerDeviceRequest = new RegisterDeviceRequest(null, "device-token", null, "en");

            response = await _client.PostAsJsonAsync("/api/DeviceRegistration", registerDeviceRequest);

            response.AssertHttpStatusCode(HttpStatusCode.BadRequest);
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

        #endregion
        
        #region DELETE /api/DeviceRegistration

        [Fact]
        public async Task UnregisterDevice_WithNullOrEmptyDeviceToken_ShouldReturnBadRequest()
        {
            var unregisterDeviceRequest = new UnregisterDeviceRequest("");
            
            var response = await _client.DeleteWithPayloadAsync("/api/DeviceRegistration", unregisterDeviceRequest);

            response.AssertHttpStatusCode(HttpStatusCode.BadRequest);

            unregisterDeviceRequest = new UnregisterDeviceRequest(null);

            response = await _client.DeleteWithPayloadAsync("/api/DeviceRegistration", unregisterDeviceRequest);

            response.AssertHttpStatusCode(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task UnregisterDevice_WithDeviceTokenThatDoesntExist_ShouldReturnDoesNotExist()
        {
            var unregisterDeviceRequest = new UnregisterDeviceRequest("device-token-that-does-not-exist");
            
            var response = await _client.DeleteWithPayloadAsync("/api/DeviceRegistration", unregisterDeviceRequest);
            
            await response.AssertErrorAsync(DeviceRegistrationErrorCode.DoesNotExist);
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
