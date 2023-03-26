using System.Collections.Generic;

namespace Lykke.Snow.Notifications.DomainServices.Model
{
    public class LocalizationData
    {
        public IReadOnlyDictionary<string, IReadOnlyDictionary<string, string>> Titles { get; set; }
        public IReadOnlyDictionary<string, IReadOnlyDictionary<string, string>> Bodies { get; set; }
    }

}
