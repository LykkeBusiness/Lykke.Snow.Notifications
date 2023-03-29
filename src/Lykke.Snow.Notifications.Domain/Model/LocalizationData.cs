using System.Collections.Generic;
using Lykke.Snow.Notifications.Domain.Exceptions;

namespace Lykke.Snow.Notifications.Domain.Model
{
    public class LocalizationData
    {
        public IReadOnlyDictionary<string, IReadOnlyDictionary<string, string>> Titles { get; }
        public IReadOnlyDictionary<string, IReadOnlyDictionary<string, string>> Bodies { get; }
        
        public LocalizationData(IReadOnlyDictionary<string, IReadOnlyDictionary<string, string>> titles, IReadOnlyDictionary<string, IReadOnlyDictionary<string, string>> bodies)
        {
            if(titles == null || titles.Count == 0)
                throw new LocalizationFileParsingException();

            if(bodies == null || bodies.Count == 0)
                throw new LocalizationFileParsingException();
            
            Titles = titles;
            Bodies = bodies;
        }
    }
}
