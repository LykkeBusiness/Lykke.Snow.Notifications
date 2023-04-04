using System;

namespace Lykke.Snow.Notifications.Domain.Model
{
    public sealed class DeviceRegistration : IEquatable<DeviceRegistration>
    {
        public int Oid { get; set; }
        public string AccountId { get; }
        public string DeviceToken { get; }
        public string DeviceId { get; }
        public DateTime RegisteredOn { get; }

        public DeviceRegistration(string accountId, string deviceToken, string deviceId, DateTime registeredOn) 
        {
            if(string.IsNullOrEmpty(accountId))
                throw new ArgumentNullException(nameof(accountId));

            if(string.IsNullOrEmpty(deviceToken))
                throw new ArgumentNullException(nameof(deviceToken));

            if(string.IsNullOrEmpty(deviceId))
                throw new ArgumentNullException(nameof(deviceId));
            
            if(registeredOn == default)
                throw new ArgumentException("Value cannot be default.", nameof(registeredOn));
        
            if(registeredOn > DateTime.UtcNow)
                throw new ArgumentException($"{nameof(registeredOn)} value cannot be in the future.");
            
            AccountId = accountId;
            DeviceToken = deviceToken;
            RegisteredOn = registeredOn;
            DeviceId = deviceId;
        }

        public override string ToString()
        {
            return $"AccountId: {AccountId}, " +
                   $"DeviceToken: {DeviceToken}, " +
                   $"DeviceId: {DeviceId}, " +
                   $"RegisteredOn: {RegisteredOn}";
        }

        public bool Equals(DeviceRegistration? other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return AccountId == other.AccountId &&
                   DeviceToken == other.DeviceToken &&
                   DeviceId == other.DeviceId &&
                   RegisteredOn.Equals(other.RegisteredOn);
        }

        public override bool Equals(object? obj)
        {
            return ReferenceEquals(this, obj) || obj is DeviceRegistration other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(AccountId, DeviceToken, DeviceId, RegisteredOn);
        }
    }
}
