using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Snow.Contracts.Responses;
using Lykke.Snow.Notifications.Client.Model;
using Lykke.Snow.Notifications.Client.Model.Requests;
using Refit;

namespace Lykke.Snow.Notifications.Client
{
    /// <summary>
    /// Notifications client API interface.
    /// </summary>
    [PublicAPI]
    public interface INotificationsApi
    {
        /// <summary>
        /// The endpoint that'll be used to store FCM token of the mobile device.
        /// This endpoint should be called upon FCM SDK initialization on mobile device.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Post("/api/DeviceRegistration/register")]
        Task<ErrorCodeResponse<DeviceRegistrationErrorCodeContract>> RegisterDevice([Body] RegisterDeviceRequest request);

        /// <summary>
        /// The endpoint that'll be used to remove FCM token of the mobile device from the database.
        /// This endpoint should be called when FCM device token becomes stale.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Post("/api/DeviceRegistration/unregister")]
        Task<ErrorCodeResponse<DeviceRegistrationErrorCodeContract>> UnregisterDevice([Body] UnregisterDeviceRequest request);
    }
}
