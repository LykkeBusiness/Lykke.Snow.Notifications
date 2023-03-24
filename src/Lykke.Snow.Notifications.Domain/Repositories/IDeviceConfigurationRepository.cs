using System.Threading.Tasks;
using Lykke.Snow.Notifications.Domain.Model;

namespace Lykke.Snow.Notifications.Domain.Repositories
{
    public interface IDeviceConfigurationRepository
    {
        Task<DeviceConfiguration> GetAsync(string deviceId);
        Task AddOrUpdateAsync(DeviceConfiguration deviceConfiguration);
        Task RemoveAsync(string deviceId);
    }
}
