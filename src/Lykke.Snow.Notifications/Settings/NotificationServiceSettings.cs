using JetBrains.Annotations;
using Lykke.SettingsReader.Attributes;
using Lykke.Snow.Common.Startup.ApiKey;

namespace Lykke.Snow.Notifications.Settings
{
    public class NotificationServiceSettings
    {
        public DbSettings Db { get; set; }

        [Optional, CanBeNull]
        public ClientSettings NotificationServiceClient { get; set; }
    }
}
