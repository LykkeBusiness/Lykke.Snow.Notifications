using System;

namespace Lykke.Snow.Notifications.Domain.Model
{
    public class DeviceRegistration
    {
        public string Id { get; set; }
        public string ClientId { get; set; }
        public string DeviceToken { get; set; }
        public DateTime RegisteredOn { get; set; }
    }
}
