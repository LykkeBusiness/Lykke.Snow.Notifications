using System;

namespace Lykke.Snow.Notifications.Domain.Exceptions
{
    public class NotificationTypeConversionException : Exception
    {
        public NotificationTypeConversionException(string message) : base(message)
        {
        }
    }
}
