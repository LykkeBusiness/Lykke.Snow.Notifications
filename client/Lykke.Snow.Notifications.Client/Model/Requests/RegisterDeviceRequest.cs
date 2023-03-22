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
        public string AccountId { get; }

        /// <summary>
        /// FCM registration token
        /// </summary>
        /// <value></value>
        [Required]
        public string DeviceToken { get; }
        
        
        /// <summary>
        /// Constructor for RegisterDeviceRequest
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="deviceToken"></param>
        public RegisterDeviceRequest(string accountId, string deviceToken)
        {
            DeviceToken = deviceToken;
            AccountId = accountId;
        }
    }
}
