using System;

namespace Lykke.Snow.Notifications.Domain.Model
{
    public class DeviceRegistration
    {
        public string ClientId { get; set; }
        public string DeviceToken { get; set; }
        public DateTime RegisteredOn { get; set; }
        
        public DeviceRegistration(string clientId, string deviceToken)
        {
            ClientId = clientId;
            DeviceToken = deviceToken;
            RegisteredOn = DateTime.UtcNow; //TODO: check if we can get in any other way
        }
    }
}
