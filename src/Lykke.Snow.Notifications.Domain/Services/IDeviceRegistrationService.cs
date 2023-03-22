using System.Collections.Generic;
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
        /// <param name="deviceToken"></param>
        /// <param name="accountId"></param>
        /// <returns></returns>
        Task<Result<DeviceRegistrationErrorCode>> UnregisterDeviceAsync(string deviceToken, string accountId);
        
        /// <summary>
        /// Get all device registrations associated by given accountId
        /// Intented to be used to send notifications when a client is logged in multiple devices
        /// </summary>
        /// <param name="accountId"></param>
        /// <returns>An IEnumerable<> to the iterate the collection of device registrations. Returns empty collection if there's no any.</returns>
        Task<Result<IEnumerable<DeviceRegistration>, DeviceRegistrationErrorCode>> GetDeviceRegistrationsAsync(string accountId);
    }
}
