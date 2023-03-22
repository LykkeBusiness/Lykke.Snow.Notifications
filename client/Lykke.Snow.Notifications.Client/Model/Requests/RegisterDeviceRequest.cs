using System.ComponentModel.DataAnnotations;

namespace Lykke.Snow.Notifications.Client.Model.Requests
{
    /// <summary>
    /// Device registration request
    /// </summary>
    public class RegisterDeviceRequest
    {
        /// <summary>
        /// AccountId that will be associated with the device
        /// </summary>
        /// <value></value>
        [Required]
        public string AccountId { get; set; }

        /// <summary>
        /// FCM registration token
        /// </summary>
        /// <value></value>
        [Required]
        public string DeviceToken { get; set; }
    }
}
