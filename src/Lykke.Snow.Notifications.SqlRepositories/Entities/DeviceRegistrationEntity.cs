using System;

namespace Lykke.Snow.Notifications.SqlRepositories.Entities
{
    public class DeviceRegistrationEntity
    {
        public string AccountId { get; set; }
        public string DeviceToken { get; set; }
        public DateTime RegisteredOn { get; set; }
    }
}
