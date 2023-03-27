using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Lykke.Snow.Contracts.Responses;
using Lykke.Snow.Notifications.Client.Models;
using Lykke.Snow.Notifications.Domain.Model;
using Newtonsoft.Json;
using Xunit;

namespace Lykke.Snow.Notifications.Tests
{
    internal static class DeviceConfigurationTestExtensions
    {
        internal static async Task<T> ReadAsAsync<T>(this HttpContent content) where T : class
        {
            var json = await content.ReadAsStringAsync();
            if (string.IsNullOrEmpty(json))
                throw new InvalidOperationException("Content is empty");

            var result = JsonConvert.DeserializeObject<T>(json);
            if (result == null)
                throw new InvalidOperationException("Content is not a valid JSON");

            return result;
        }

        public static async Task AssertSuccessAsync(this HttpResponseMessage response)
        {
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var result = await response.Content.ReadAsAsync<ErrorCodeResponse<DeviceConfigurationErrorCodeContract>>();
            Assert.Equal(DeviceConfigurationErrorCodeContract.None, result.ErrorCode);
        }

        public static async Task AssertErrorAsync(this HttpResponseMessage response,
            DeviceConfigurationErrorCodeContract errorCode)
        {
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var result = await response.Content.ReadAsAsync<ErrorCodeResponse<DeviceConfigurationErrorCodeContract>>();
            Assert.Equal(errorCode, result.ErrorCode);
        }

        public static async Task AssertAsync<T>(this HttpResponseMessage response, Action<T> assert) where T : class
        {
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var result = await response.Content.ReadAsAsync<T>();
            assert(result);
        }
    }

    public class DeviceConfigurationControllerIntegrationTests : IClassFixture<WebAppFactoryNoDependencies>
    {
        private readonly HttpClient _client;

        public DeviceConfigurationControllerIntegrationTests(WebAppFactoryNoDependencies factory)
        {
            _client = factory.CreateSecuredClient();
        }

        [Fact]
        public async Task Get_ReturnsNotFound_ForNonExistentDevice()
        {
            var response = await _client.GetAsync("/api/DeviceConfiguration/NonExistentDevice");
            await response.AssertErrorAsync(DeviceConfigurationErrorCodeContract.DoesNotExist);
        }

        [Fact]
        public async Task AddOrUpdateAndGetDeviceConfiguration_ReturnsCreatedAndRetrievedDeviceConfiguration()
        {
            var deviceConfiguration = CreateSampleDeviceConfiguration();

            // Add a new device configuration
            var addOrUpdateResponse = await _client.PostAsJsonAsync("/api/DeviceConfiguration", deviceConfiguration);
            await addOrUpdateResponse.AssertSuccessAsync();

            // Get the added device configuration
            var getResponse = await _client.GetAsync($"/api/DeviceConfiguration/{deviceConfiguration.DeviceId}");
            await getResponse.AssertAsync<DeviceConfigurationResponse>(result =>
            {
                Assert.Equal(DeviceConfigurationErrorCodeContract.None, result.ErrorCode);
                Assert.Equal(deviceConfiguration.DeviceId, result.DeviceConfiguration?.DeviceId);
                Assert.Equal(deviceConfiguration.AccountId, result.DeviceConfiguration?.AccountId);
                Assert.Equal(deviceConfiguration.Locale, result.DeviceConfiguration?.Locale);
                Assert.Equal(deviceConfiguration.NotificationsOn, result.DeviceConfiguration?.NotificationsOn);
            });
        }

        [Fact]
        public async Task Delete_ReturnsNotFound_ForNonExistentDevice()
        {
            var response = await _client.DeleteAsync("/api/DeviceConfiguration/NonExistentDevice");
            await response.AssertErrorAsync(DeviceConfigurationErrorCodeContract.DoesNotExist);
        }

        [Fact]
        public async Task AddOrUpdateAndDeleteDeviceConfiguration_ReturnsCreatedAndDeletedDeviceConfiguration()
        {
            var deviceConfiguration = CreateSampleDeviceConfiguration();

            // Add a new device configuration
            var addOrUpdateResponse = await _client.PostAsJsonAsync("/api/DeviceConfiguration", deviceConfiguration);
            await addOrUpdateResponse.AssertSuccessAsync();

            // Delete the added device configuration
            var deleteResponse = await _client.DeleteAsync($"/api/DeviceConfiguration/{deviceConfiguration.DeviceId}");
            await deleteResponse.AssertSuccessAsync();

            // Check if the device configuration is deleted
            var getResponse = await _client.GetAsync($"/api/DeviceConfiguration/{deviceConfiguration.DeviceId}");
            await getResponse.AssertErrorAsync(DeviceConfigurationErrorCodeContract.DoesNotExist);
        }

        [Fact]
        public async Task AddOrUpdate_ReturnsInvalidInput_ForInvalidDeviceConfiguration()
        {
            var invalidDeviceConfiguration = new DeviceConfigurationContract
            {
                DeviceId = "TestDevice",
                AccountId = "TestAccount",
                Locale = "en",
                NotificationsOn = new[] { "InvalidType" }
            };

            var response = await _client.PostAsJsonAsync("/api/DeviceConfiguration", invalidDeviceConfiguration);
            await response.AssertErrorAsync(DeviceConfigurationErrorCodeContract.InvalidInput);
        }

        [Fact]
        public async Task AddOrUpdate_Updates_ForExistingDeviceConfiguration()
        {
            var deviceConfiguration = CreateSampleDeviceConfiguration();

            // Add a new device configuration
            var addOrUpdateResponse = await _client.PostAsJsonAsync("/api/DeviceConfiguration", deviceConfiguration);
            await addOrUpdateResponse.AssertSuccessAsync();

            // Try to add the same device configuration again
            var addOrUpdateAgainResponse =
                await _client.PostAsJsonAsync("/api/DeviceConfiguration", deviceConfiguration);
            await addOrUpdateAgainResponse.AssertSuccessAsync();
        }

        [Fact]
        public async Task AddOrUpdate_ReturnsConflict_ConcurrentRequests()
        {
            const int numberOfConcurrentRequests = 100;

            var tasks = new Task<DeviceConfigurationErrorCodeContract>[numberOfConcurrentRequests];

            for (var i = 0; i < numberOfConcurrentRequests; i++)
            {
                tasks[i] = AddDeviceConfiguration();
            }

            await Task.WhenAll(tasks);

            var errorCodes = tasks.Select(t => t.Result);

            Assert.Contains(DeviceConfigurationErrorCodeContract.Conflict, errorCodes);

            async Task<DeviceConfigurationErrorCodeContract> AddDeviceConfiguration()
            {
                var deviceConfiguration = CreateSampleDeviceConfiguration();

                var addOrUpdateResponse =
                    await _client.PostAsJsonAsync("/api/DeviceConfiguration", deviceConfiguration);
                var addOrUpdateResult = await addOrUpdateResponse.Content
                    .ReadAsAsync<ErrorCodeResponse<DeviceConfigurationErrorCodeContract>>();
                return addOrUpdateResult.ErrorCode;
            }
        }

        private static DeviceConfigurationContract CreateSampleDeviceConfiguration()
        {
            return new DeviceConfigurationContract
            {
                DeviceId = "TestDevice",
                AccountId = "TestAccount",
                Locale = "en",
                NotificationsOn = new[] { NotificationType.DepositSucceeded.ToString() }
            };
        }
    }
}
