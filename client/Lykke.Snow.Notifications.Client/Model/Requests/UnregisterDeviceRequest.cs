using System.ComponentModel.DataAnnotations;

namespace Lykke.Snow.Notifications.Client.Model.Requests
{
    /// <summary>
    /// Device unregistration request
    /// </summary>
    public class UnregisterDeviceRequest
    {
        /// <summary>
        /// Account id that's been associated with the device.
        /// </summary>
        /// <value></value>
        [Required]
        public string AccountId { get; set; }

        /// <summary>
        /// Device token to be unregistered.
        /// </summary>
        /// <value></value>
        [Required]
        public string DeviceToken { get; set; }
    }
}
