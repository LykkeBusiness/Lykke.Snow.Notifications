using System;
using Lykke.SettingsReader.Attributes;

namespace Lykke.Snow.Notifications.Settings
{
    public class CqrsSettings
    {
        [AmqpCheck] public string ConnectionString { get; set; } = null!;

        public TimeSpan RetryDelay { get; set; }
        [Optional] public string? EnvironmentName { get; set; }
        [Optional] public CqrsContextNamesSettings ContextNames { get; set; } = new CqrsContextNamesSettings();
    }
}
