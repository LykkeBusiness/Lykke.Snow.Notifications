using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Snow.Notifications.Domain.Model;

namespace Lykke.Snow.Notifications.Domain.Repositories
{
    public interface IDeviceRegistrationRepository
    {
        Task<IReadOnlyList<DeviceRegistration>> GetDeviceRegistrationsAsync(string deviceToken);
        Task<IReadOnlyList<DeviceRegistration>> GetDeviceRegistrationsByAccountIdAsync(string accountId);
        Task<IReadOnlyList<DeviceRegistration>> GetDeviceRegistrationsByAccountIdsAsync(string[] accountIds);
        Task AddOrUpdateAsync(DeviceRegistration model);
        Task RemoveAsync(int oid);
        Task RemoveAllAsync(int[] oids);
    }
}
