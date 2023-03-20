using System;
using Microsoft.Extensions.Internal;

namespace Lykke.Snow.Notifications.Domain.Model
{
    public class DeviceRegistration
    {
        public string AccountId { get; }
        public string DeviceToken { get; }
        public DateTime RegisteredOn { get; private set; }
        
        public DeviceRegistration(string accountId, string deviceToken) {
            if(string.IsNullOrEmpty(accountId))
                throw new ArgumentNullException(nameof(accountId));

            if(string.IsNullOrEmpty(deviceToken))
                throw new ArgumentNullException(nameof(deviceToken));

            AccountId = accountId;
            DeviceToken = deviceToken;
        }
        
        public void SetRegisteredOn(ISystemClock systemClock)
        {
            RegisteredOn = systemClock.UtcNow.DateTime;
        }
    }
}
