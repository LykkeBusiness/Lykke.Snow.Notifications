using System.ComponentModel.DataAnnotations;

namespace Lykke.Snow.Notifications.Contracts.Model.Requests
{
    public class UnregisterDeviceRequest
    {
        [Required]
        public string AccountId { get; set; }

        [Required]
        public string DeviceToken { get; set; }
    }
}
