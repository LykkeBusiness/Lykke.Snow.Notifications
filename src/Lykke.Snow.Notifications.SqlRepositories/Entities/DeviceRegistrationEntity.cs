using System;

namespace Lykke.Snow.Notifications.SqlRepositories.Entities
{
    /// <summary>
    /// Device registration in Firebase Cloud Messaging (FCM)
    /// </summary>
    public class DeviceRegistrationEntity
    {
        public int Oid { get; set; }
        public string AccountId { get; set; }
        public string DeviceToken { get; set; }
        public string DeviceId { get; set; } 
        public DateTime RegisteredOn { get; set; }
    }
}
