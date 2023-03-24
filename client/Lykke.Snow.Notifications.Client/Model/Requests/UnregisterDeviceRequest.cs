using System.ComponentModel.DataAnnotations;

namespace Lykke.Snow.Notifications.Client.Model.Requests
{
    /// <summary>
    /// Device unregistration request
    /// </summary>
    public class UnregisterDeviceRequest
    {
        /// <summary>
        /// Device token to be unregistered.
        /// </summary>
        /// <value></value>
        [Required]
        public string DeviceToken { get; set; }
    }
}
