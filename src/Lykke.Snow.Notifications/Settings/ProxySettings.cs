using Lykke.SettingsReader.Attributes;

namespace Lykke.Snow.Notifications.Settings
{
    public class ProxySettings
    {
        public string Address { get; set; } = "";
        
        [Optional]
        public string? Username { get; set; }
        
        [Optional]
        public string? Password { get; set; }
    }
}
