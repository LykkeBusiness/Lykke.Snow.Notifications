using System;
using Lykke.SettingsReader.Attributes;

namespace Lykke.Snow.Notifications.Settings
{
    public class CacheSettings
    {
        [Optional] public TimeSpan? ExpirationPeriod { get; set; }
    }
}
