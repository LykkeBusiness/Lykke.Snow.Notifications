using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Snow.Contracts.Responses;
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
        /// <returns></returns>
        [Get("/api/DeviceConfiguration/{deviceId}")]
        Task<DeviceConfigurationResponse> Get([Required] string deviceId);

        /// <summary>
        /// Removes device configuration by device id
        /// </summary>
        /// <param name="deviceId"></param>
        /// <returns></returns>
        [Delete("/api/DeviceConfiguration/{deviceId}")]
        Task<ErrorCodeResponse<DeviceConfigurationErrorCodeContract>> Delete([Required] string deviceId);

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
