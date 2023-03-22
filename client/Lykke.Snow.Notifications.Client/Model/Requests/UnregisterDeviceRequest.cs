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
        
        /// <summary>
        /// Constructor for UnregisterDeviceRequest
        /// </summary>
        /// <param name="deviceToken"></param>
        public UnregisterDeviceRequest(string deviceToken)
        {
            DeviceToken = deviceToken;
        }
    }
}
