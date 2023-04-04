using Lykke.SettingsReader.Attributes;
using Lykke.Snow.Common.Startup.ApiKey;

namespace Lykke.Snow.Notifications.Settings
{
    public class NotificationServiceSettings
    {
        public DbSettings Db { get; set; } = new DbSettings();

        [Optional]
        public ClientSettings? NotificationServiceClient { get; set; }

        public CqrsSettings Cqrs { get; set; } = new CqrsSettings();
        public FcmSettings Fcm { get; set; } = new FcmSettings();
        public SubscribersSettings? Subscribers { get; set; }
        
        public CacheSettings? ConfigurationCache { get; set; }
    }
}
