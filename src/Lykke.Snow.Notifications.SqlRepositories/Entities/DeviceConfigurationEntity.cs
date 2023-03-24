using System;
using System.Collections.Generic;

namespace Lykke.Snow.Notifications.SqlRepositories.Entities
{
    /// <summary>
    /// Device notifications configuration
    /// </summary>
    public class DeviceConfigurationEntity
    {
        public int Oid { get; set; }
        public string DeviceId { get; set; }
        public string AccountId { get; set; }
        
        public string Locale { get; set; }
        public DateTime CreatedOn { get; set; }
        public ICollection<DeviceNotificationConfigurationEntity> Notifications { get; set; }
    }
}
