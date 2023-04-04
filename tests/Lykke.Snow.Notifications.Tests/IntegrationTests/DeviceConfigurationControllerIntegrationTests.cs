using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Lykke.Snow.Contracts.Responses;
using Lykke.Snow.Notifications.Client.Models;
using Lykke.Snow.Notifications.Domain.Enums;
using Lykke.Snow.Notifications.Tests.Extensions;
using Xunit;

namespace Lykke.Snow.Notifications.Tests.IntegrationTests
{
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
            var response = await _client.GetAsync("/api/DeviceConfiguration/NonExistentDevice/NonExistentAccount");
            await response.AssertErrorAsync(DeviceConfigurationErrorCodeContract.DoesNotExist);
        }

        [Fact]
        public async Task AddOrUpdateAndGetDeviceConfiguration_ReturnsCreatedAndRetrievedDeviceConfiguration()
        {
            var deviceConfiguration = CreateSampleDeviceConfiguration();

            // Add a new device configuration
            var addOrUpdateResponse = await _client.PostAsJsonAsync("/api/DeviceConfiguration", deviceConfiguration);
            await addOrUpdateResponse.AssertSuccessAsync(DeviceConfigurationErrorCodeContract.None);

            // Get the added device configuration
            var getResponse = await _client.GetAsync($"/api/DeviceConfiguration/{deviceConfiguration.DeviceId}/{deviceConfiguration.AccountId}");
            await getResponse.AssertAsync<DeviceConfigurationResponse>(result =>
            {
                Assert.Equal(DeviceConfigurationErrorCodeContract.None, result.ErrorCode);
                Assert.Equal(deviceConfiguration.DeviceId, result.DeviceConfiguration?.DeviceId);
                Assert.Equal(deviceConfiguration.AccountId, result.DeviceConfiguration?.AccountId);
                Assert.Equal(deviceConfiguration.Locale, result.DeviceConfiguration?.Locale, StringComparer.OrdinalIgnoreCase);
                Assert.Equal(deviceConfiguration.NotificationsOn, result.DeviceConfiguration?.NotificationsOn);
            });
        }

        [Fact]
        public async Task Delete_ReturnsNotFound_ForNonExistentDevice()
        {
            var response = await _client.DeleteAsync("/api/DeviceConfiguration/NonExistentDevice/NonExistentAccount");
            await response.AssertErrorAsync(DeviceConfigurationErrorCodeContract.DoesNotExist);
        }

        [Fact]
        public async Task AddOrUpdateAndDeleteDeviceConfiguration_ReturnsCreatedAndDeletedDeviceConfiguration()
        {
            var deviceConfiguration = CreateSampleDeviceConfiguration();

            // Add a new device configuration
            var addOrUpdateResponse = await _client.PostAsJsonAsync("/api/DeviceConfiguration", deviceConfiguration);
            await addOrUpdateResponse.AssertSuccessAsync(DeviceConfigurationErrorCodeContract.None);

            // Delete the added device configuration
            var deleteResponse = await _client.DeleteAsync($"/api/DeviceConfiguration/{deviceConfiguration.DeviceId}/{deviceConfiguration.AccountId}");
            await deleteResponse.AssertSuccessAsync(DeviceConfigurationErrorCodeContract.None);

            // Check if the device configuration is deleted
            var getResponse = await _client.GetAsync($"/api/DeviceConfiguration/{deviceConfiguration.DeviceId}/{deviceConfiguration.AccountId}");
            await getResponse.AssertErrorAsync(DeviceConfigurationErrorCodeContract.DoesNotExist);
        }

        [Theory]
        [InlineData(null, DeviceConfigurationErrorCodeContract.InvalidInput)]
        [InlineData("", DeviceConfigurationErrorCodeContract.InvalidInput)]
        [InlineData(" ", DeviceConfigurationErrorCodeContract.InvalidInput)]
        [InlineData("InvalidNotificationType", DeviceConfigurationErrorCodeContract.UnsupportedNotificationType)]
        public async Task AddOrUpdate_ReturnsError_ForInvalidNotificationType(string notificationType,
            DeviceConfigurationErrorCodeContract errorCode)
        {
            var invalidDeviceConfiguration = new DeviceConfigurationContract
            {
                DeviceId = "TestDevice",
                AccountId = "TestAccount",
                Locale = "en",
                NotificationsOn = new[] { notificationType }
            };

            var response = await _client.PostAsJsonAsync("/api/DeviceConfiguration", invalidDeviceConfiguration);
            await response.AssertErrorAsync(errorCode);
        }

        [Fact]
        public async Task AddOrUpdate_ReturnsError_ForInvalidLocale()
        {
            var invalidDeviceConfiguration = new DeviceConfigurationContract
            {
                DeviceId = "TestDevice",
                AccountId = "TestAccount",
                Locale = "UnsupportedLocale",
                NotificationsOn = new[] { NotificationType.DepositSucceeded.ToString() }
            };

            var response = await _client.PostAsJsonAsync("/api/DeviceConfiguration", invalidDeviceConfiguration);
            await response.AssertErrorAsync(DeviceConfigurationErrorCodeContract.UnsupportedLocale);
        }

        [Fact]
        public async Task AddOrUpdate_Updates_ForExistingDeviceConfiguration()
        {
            var deviceConfiguration = CreateSampleDeviceConfiguration();

            // Add a new device configuration
            var addOrUpdateResponse = await _client.PostAsJsonAsync("/api/DeviceConfiguration", deviceConfiguration);
            await addOrUpdateResponse.AssertSuccessAsync(DeviceConfigurationErrorCodeContract.None);

            // Try to add the same device configuration again
            var addOrUpdateAgainResponse =
                await _client.PostAsJsonAsync("/api/DeviceConfiguration", deviceConfiguration);
            await addOrUpdateAgainResponse.AssertSuccessAsync(DeviceConfigurationErrorCodeContract.None);
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
