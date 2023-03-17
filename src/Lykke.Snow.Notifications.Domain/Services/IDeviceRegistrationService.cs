using System.Threading.Tasks;
using Lykke.Snow.Common.Model;
using Lykke.Snow.Notifications.Domain.Enums;
using Lykke.Snow.Notifications.Domain.Model;

namespace Lykke.Snow.Notifications.Domain.Services
{
    /// <summary>
    /// Device registration is the process of correlating particular client with a unique mobile device.
    /// This service that's responsible for handling device token registrations
    /// </summary>
    public interface IDeviceRegistrationService
    {
        /// <summary>
        /// Registers the device.
        /// </summary>
        /// <param name="deviceRegistration"></param>
        /// <returns></returns>
        Task<Result<DeviceRegistrationErrorCode>> RegisterDeviceAsync(DeviceRegistration deviceRegistration);
        
        /// <summary>
        /// Unregisters the device.
        /// </summary>
        /// <param name="deviceRegistration"></param>
        /// <returns></returns>
        Task<Result<DeviceRegistrationErrorCode>> UnregisterDeviceAsync(DeviceRegistration deviceRegistration);
    }
}
