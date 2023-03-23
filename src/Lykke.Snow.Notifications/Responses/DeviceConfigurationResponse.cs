using System.Collections.Generic;

namespace Lykke.Snow.Notifications.Responses
{
    public class DeviceConfigurationResponse
    {
        public string DeviceId { get; set; }
        public string AccountId { get; set; }
        public string Locale { get; set; }
        public List<NotificationResponse> Notifications { get; set; }
    }
}
