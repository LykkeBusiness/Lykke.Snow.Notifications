using System;

namespace Lykke.Snow.Notifications.Domain.Exceptions
{
    public sealed class UnsupportedNotificationTypeException : Exception 
    {
        public UnsupportedNotificationTypeException(string type) : base($"Notification type [{type}] is not supported")
        {
            Data.Add("Type", type);
        }
    }
}
