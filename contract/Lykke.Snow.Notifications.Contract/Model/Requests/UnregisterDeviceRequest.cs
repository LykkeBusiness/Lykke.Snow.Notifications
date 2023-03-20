using System.ComponentModel.DataAnnotations;

namespace Lykke.Snow.Notifications.Contracts.Model.Requests
{
    public class UnregisterDeviceRequest
    {
        [Required]
        public string DeviceId { get; set; }

        [Required]
        public string ClientId { get; set; }
        
        public UnregisterDeviceRequest(string deviceId, string clientId)
        {
            this.DeviceId = deviceId;
            this.ClientId = clientId; 
        }
    }
}
