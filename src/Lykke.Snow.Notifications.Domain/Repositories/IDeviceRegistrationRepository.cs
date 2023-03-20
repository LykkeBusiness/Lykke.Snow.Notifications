using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Snow.Common.Model;
using Lykke.Snow.Notifications.Domain.Enums;
using Lykke.Snow.Notifications.Domain.Model;

namespace Lykke.Snow.Notifications.Domain.Repositories
{
    public interface IDeviceRegistrationRepository
    {
        Task<Result<DeviceRegistration, DeviceRegistrationErrorCode>> GetDeviceRegistrationAsync(string deviceToken);
        Task<Result<IEnumerable<DeviceRegistration>, DeviceRegistrationErrorCode>> GetDeviceRegistrationsByAccountIdAsync(string accountId);
        Task<Result<DeviceRegistrationErrorCode>> InsertAsync(DeviceRegistration entity);
        Task<Result<DeviceRegistrationErrorCode>> DeleteAsync(string deviceToken);
    }
}
