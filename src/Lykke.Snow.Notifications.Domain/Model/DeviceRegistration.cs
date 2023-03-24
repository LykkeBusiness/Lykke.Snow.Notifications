using System;

namespace Lykke.Snow.Notifications.Domain.Model
{
    public sealed class DeviceRegistration
    {
        public string AccountId { get; }
        public string DeviceToken { get; }
        public DateTime RegisteredOn { get; }

        public DeviceRegistration(string accountId, string deviceToken, DateTime registeredOn) 
        {
            if(string.IsNullOrEmpty(accountId))
                throw new ArgumentNullException(nameof(accountId));

            if(string.IsNullOrEmpty(deviceToken))
                throw new ArgumentNullException(nameof(deviceToken));
            
            if(registeredOn == default(DateTime))
                throw new ArgumentException();
        
            if(registeredOn > DateTime.UtcNow)
                throw new ArgumentException($"{nameof(registeredOn)} value cannot be in the future.");
            
            AccountId = accountId;
            DeviceToken = deviceToken;
            RegisteredOn = registeredOn;
        }
    }
}
