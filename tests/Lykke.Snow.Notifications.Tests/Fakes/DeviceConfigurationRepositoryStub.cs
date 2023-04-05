using System.Threading.Tasks;
using Lykke.Snow.Notifications.Domain.Model;
using Lykke.Snow.Notifications.Domain.Repositories;

namespace Lykke.Snow.Notifications.Tests.Fakes
{
    public class DeviceConfigurationRepositoryStub : IDeviceConfigurationRepository
    {
        public int NumOfGetAsyncCalls { get; private set; }

        public Task AddOrUpdateAsync(DeviceConfiguration deviceConfiguration)
        {
            throw new System.NotImplementedException();
        }

        public Task<DeviceConfiguration> GetAsync(string deviceId, string accountId)
        {
            NumOfGetAsyncCalls++;
            return Task.FromResult(new DeviceConfiguration(deviceId, "any-account-id"));
        }

        public Task RemoveAsync(string deviceId, string accountId)
        {
            throw new System.NotImplementedException();
        }
    }
}
