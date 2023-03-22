using System.Threading.Tasks;
using DeviceConfiguration = Lykke.Snow.Notifications.Domain.Repositories.DeviceConfigurationStub;

namespace Lykke.Snow.Notifications.Domain.Repositories
{
    public class DeviceConfigurationStub
    {
        
    }
    
    public interface IDeviceConfigurationRepository
    {
        Task<DeviceConfiguration?> GetByIdAsync(int id);
        Task AddAsync(DeviceConfiguration deviceConfiguration);
        Task UpdateAsync(DeviceConfiguration deviceConfiguration);
        Task RemoveAsync(int id);
    }
}
