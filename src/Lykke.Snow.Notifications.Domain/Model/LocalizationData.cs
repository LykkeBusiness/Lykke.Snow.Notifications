using System.Collections.Generic;
using Lykke.Snow.Notifications.Domain.Exceptions;

namespace Lykke.Snow.Notifications.Domain.Model
{
    public class LocalizationData
    {
        public IReadOnlyDictionary<string, IReadOnlyDictionary<string, string>> Titles { get; }
        public IReadOnlyDictionary<string, IReadOnlyDictionary<string, string>> Bodies { get; }
        public IReadOnlyDictionary<string, IReadOnlyDictionary<string, string>> Attributes { get; }
        
        public LocalizationData(IReadOnlyDictionary<string, IReadOnlyDictionary<string, string>> titles, 
            IReadOnlyDictionary<string, IReadOnlyDictionary<string, string>> bodies,
            IReadOnlyDictionary<string, IReadOnlyDictionary<string, string>> attributes)
        {
            if(titles == null || titles.Count == 0)
                throw new LocalizationFileParsingException();

            if(bodies == null || bodies.Count == 0)
                throw new LocalizationFileParsingException();
            
            if(attributes == null || attributes.Count == 0)
                throw new LocalizationFileParsingException();
            
            Titles = titles;
            Bodies = bodies;
            Attributes = attributes;
        }
    }
}
