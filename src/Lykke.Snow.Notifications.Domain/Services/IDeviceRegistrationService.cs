using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Snow.Common.Model;
using Lykke.Snow.Notifications.Domain.Enums;
using Lykke.Snow.Notifications.Domain.Model;

namespace Lykke.Snow.Notifications.Domain.Services
{
    /// <summary>
    /// Device registration is the process of correlating particular trading account with a unique mobile device.
    /// This service that's responsible for handling device token registrations
    /// </summary>
    public interface IDeviceRegistrationService
    {
        /// <summary>
        /// Registers the device.
        /// </summary>
        /// <param name="deviceRegistration"></param>
        /// <param name="locale"></param>
        /// <returns></returns>
        Task<Result<DeviceRegistrationErrorCode>> RegisterDeviceAsync(DeviceRegistration deviceRegistration,
            string locale);

        /// <summary>
        /// Unregisters the device for all accounts.
        /// </summary>
        /// <param name="deviceToken"></param>
        /// <returns></returns>
        Task<Result<DeviceRegistrationErrorCode>> UnregisterDeviceAsync(string deviceToken);

        /// <summary>
        /// Get all device registrations associated by given accountId
        /// Intended to be used to send notifications when a client is logged in multiple devices
        /// </summary>
        /// <param name="accountId"></param>
        /// <returns>
        /// An <see cref="IEnumerable{T}"/> to the iterate the collection of device registrations.
        /// Returns empty collection if there's no any.
        /// </returns>
        Task<Result<IEnumerable<DeviceRegistration>, DeviceRegistrationErrorCode>> GetDeviceRegistrationsAsync(
            string accountId);

        /// <summary>
        /// Get all device registrations by given set of account ids.
        /// Intended to be used when multiple clients are targeted.
        /// </summary>
        /// <param name="accountIds"></param>
        /// <returns>
        /// An <see cref="IEnumerable{T}"/> to the iterate the collection of device registrations.
        /// Returns empty collection if there's no any.
        /// </returns>
        Task<Result<IEnumerable<DeviceRegistration>, DeviceRegistrationErrorCode>> GetDeviceRegistrationsAsync(
            string[] accountIds);
    }
}
