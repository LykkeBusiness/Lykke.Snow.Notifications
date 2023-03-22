using System.ComponentModel.DataAnnotations;

namespace Lykke.Snow.Notifications.Contracts.Model.Requests
{
    public class RegisterDeviceRequest
    {
        [Required]
        public string AccountId { get; set; }

        [Required]
        public string DeviceToken { get; set; }
    }
}
