using Lykke.SettingsReader.Attributes;
using Lykke.Snow.Common.Startup.ApiKey;

namespace Lykke.Snow.Notifications.Settings
{
    public class NotificationServiceSettings
    {
        public DbSettings Db { get; set; }

        [Optional]
        public ClientSettings? NotificationServiceClient { get; set; }
        public CqrsSettings Cqrs { get; set; }
        public FcmSettings Fcm { get; set; }
    }
}
