using Lykke.SettingsReader.Attributes;

namespace Lykke.Snow.Notifications.Settings
{
    public class LocalizationSettings
    {
        [Optional]
        public CacheSettings? LocalizationFileCache { get; set; }
        public string LocalizationPlatformKey { get; set; } = "";
    }
}
