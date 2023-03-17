using System;
using Microsoft.Extensions.Internal;

namespace Lykke.Snow.Notifications.Domain.Model
{
    public class DeviceRegistration
    {
        public string ClientId { get; }
        public string DeviceToken { get; }
        public DateTime RegisteredOn { get; private set; }
        
        public DeviceRegistration(string clientId, string deviceToken)
        {
            if(string.IsNullOrEmpty(clientId))
                throw new ArgumentNullException(nameof(clientId));

            if(string.IsNullOrEmpty(deviceToken))
                throw new ArgumentNullException(nameof(deviceToken));

            ClientId = clientId;
            DeviceToken = deviceToken;
        }
        
        public void SetRegisteredOn(ISystemClock systemClock)
        {
            RegisteredOn = systemClock.UtcNow.DateTime;
        }
    }
}
