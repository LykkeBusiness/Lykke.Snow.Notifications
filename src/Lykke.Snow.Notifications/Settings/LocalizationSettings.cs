using Lykke.SettingsReader.Attributes;

namespace Lykke.Snow.Notifications.Settings
{
    public class LocalizationSettings
    {
        [Optional]
        public CacheSettings? LocalizationFileCache { get; set; }
        
        /// <summary>
        /// The platform key used when uploading the localization file through mdm service.
        /// </summary>
        /// <value></value>
        public string LocalizationPlatformKey { get; set; } = "";
    }
}
