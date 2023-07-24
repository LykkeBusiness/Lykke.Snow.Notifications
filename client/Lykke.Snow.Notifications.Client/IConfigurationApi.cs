using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Contracts.Responses;
using Lykke.Snow.Notifications.Client.Models;
using Refit;

namespace Lykke.Snow.Notifications.Client
{
    /// <summary>
    /// Notifications configuration API interface.
    /// </summary>
    [PublicAPI]
    public interface IConfigurationApi
    {
        /// <summary>
        /// Gets device configuration by device id
        /// </summary>
        /// <param name="deviceId">Device id</param>
        /// <param name="accountId">Account id</param>
        /// <returns></returns>
        [Get("/api/DeviceConfiguration/{deviceId}/{accountId}")]
        Task<DeviceConfigurationResponse> Get([Required] string deviceId, [Required] string accountId);

        /// <summary>
        /// Removes device configuration by device id
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="accountId"></param>
        /// <returns></returns>
        [Delete("/api/DeviceConfiguration/{deviceId}")]
        Task<ErrorCodeResponse<DeviceConfigurationErrorCodeContract>> Delete([Required] string deviceId, [Required] string accountId);

        /// <summary>
        /// Creates or updates device configuration by device id
        /// </summary>
        /// <param name="deviceConfiguration"></param>
        /// <returns></returns>
        [Post("/api/DeviceConfiguration")]
        Task<ErrorCodeResponse<DeviceConfigurationErrorCodeContract>> AddOrUpdate(
            [Body] DeviceConfigurationContract deviceConfiguration);
    }
}
