using System.ComponentModel.DataAnnotations;

namespace Lykke.Snow.Notifications.Contracts.Model.Requests
{
    public class RegisterDeviceRequest
    {
        [Required]
        public string ClientId { get; set; }

        [Required]
        public string DeviceToken { get; set; }
        
        public RegisterDeviceRequest(string clientId, string deviceToken)
        {
            ClientId = clientId;
            DeviceToken = deviceToken;
        }
    }
}
