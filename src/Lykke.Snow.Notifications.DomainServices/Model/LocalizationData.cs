using System.Collections.Generic;
using Lykke.Snow.Notifications.Domain.Exceptions;

namespace Lykke.Snow.Notifications.DomainServices.Model
{
    public class LocalizationData
    {
        public IReadOnlyDictionary<string, IReadOnlyDictionary<string, string>> Titles { get; set; }
        public IReadOnlyDictionary<string, IReadOnlyDictionary<string, string>> Bodies { get; set; }
        
        public void ThrowIfDataIsInvalid()
        {
            if(Titles == null)
                throw new LocalizationFileParsingException();

            if(Bodies == null)
                throw new LocalizationFileParsingException();
        }
    }

}
