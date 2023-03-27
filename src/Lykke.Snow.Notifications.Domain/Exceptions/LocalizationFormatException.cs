using System;

namespace Lykke.Snow.Notifications.Domain.Exceptions
{
    public class LocalizationFormatException : Exception
    {
        public LocalizationFormatException(string notificationType, string lang, string format, string[] arguments)
            : base(@$"The number of arguments in the template and given parameters don't match for {notificationType} - {lang}!
                Format: {format}
                Arguments: {string.Join(",", arguments)}") 
        {
        }
    }
}
