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

        /// <summary>
        /// Not all notification attributes should be translated.
        /// This configuration determines the attributes that are eligible for translation.
        /// </summary>
        /// <value></value>
        public string[] TranslateAttributes { get; set; } = new string[] { };
    }
}
