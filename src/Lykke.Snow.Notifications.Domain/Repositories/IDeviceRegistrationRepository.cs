using System.Threading.Tasks;
using Lykke.Snow.Common.Model;
using Lykke.Snow.Notifications.Domain.Enums;
using Lykke.Snow.Notifications.Domain.Model;

namespace Lykke.Snow.Notifications.Domain.Repositories
{
    public interface IDeviceRegistrationRepository
    {
        Task<Result<DeviceRegistration, DeviceRegistrationErrorCode>> GetDeviceRegistrationAsync(string deviceToken);
        Task<Result<DeviceRegistrationErrorCode>> InsertAsync(string deviceToken);
        Task<Result<DeviceRegistrationErrorCode>> DeleteAsync(string deviceToken, string clientId);
    }
}
