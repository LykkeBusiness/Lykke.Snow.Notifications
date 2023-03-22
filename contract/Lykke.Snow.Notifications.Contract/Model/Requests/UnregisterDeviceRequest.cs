using System.ComponentModel.DataAnnotations;

namespace Lykke.Snow.Notifications.Contracts.Model.Requests
{
    public class UnregisterDeviceRequest
    {
        [Required]
        public string AccountId { get; set; }

        [Required]
        public string DeviceToken { get; set; }
        
        public UnregisterDeviceRequest(string deviceToken, string accountId)
        {
            this.DeviceToken = deviceToken;
            this.AccountId = accountId; 
        }
    }
}
