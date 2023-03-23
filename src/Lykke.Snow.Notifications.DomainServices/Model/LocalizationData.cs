using System.Collections.Generic;

namespace Lykke.Snow.Notifications.DomainServices.Model
{
    public class LocalizationData
    {
        public IReadOnlyDictionary<string, IDictionary<string, string>> Titles { get; set; }
        public IReadOnlyDictionary<string, IDictionary<string, string>> Bodies { get; set; }
    }

}
