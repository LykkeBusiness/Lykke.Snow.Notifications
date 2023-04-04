namespace Lykke.Snow.Notifications.SqlRepositories.Entities
{
    /// <summary>
    /// Device single type of notification configuration
    /// </summary>
    public class DeviceNotificationConfigurationEntity
    {
        public int Oid { get; set; }
        public string NotificationType { get; set; }
        public bool Enabled { get; set; }
        public int DeviceConfigurationId { get; set; }
        public DeviceConfigurationEntity DeviceConfiguration { get; set; }
    }
}
