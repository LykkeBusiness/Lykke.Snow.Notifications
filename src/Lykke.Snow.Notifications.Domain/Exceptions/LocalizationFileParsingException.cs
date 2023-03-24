using System;

namespace Lykke.Snow.Notifications.Domain.Exceptions
{
    public class LocalizationFileParsingException : Exception
    {
        private const string ErrorMsg = @"Could not parse the localization file. 
            Please make sure it's a valid json file and it respects the LocalizationData format.";

        public LocalizationFileParsingException()
        {
        }
    }
}
