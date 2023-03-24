using System;

namespace Lykke.Snow.Notifications.Domain.Exceptions
{
    public class LocalizationFormatException : Exception
    {
        public LocalizationFormatException(string notificationType)
            : base($"The number of arguments in the template and given parameters don't match for {notificationType}!") 
        {
        }
    }
}
