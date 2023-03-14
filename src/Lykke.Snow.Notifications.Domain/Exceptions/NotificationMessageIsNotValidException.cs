using System;
using Lykke.Snow.Notifications.Domain.Model;

namespace Lykke.Snow.Notifications.Domain.Exceptions
{
    public class NotificationMessageIsNotValidException : Exception
    {
        public NotificationMessageIsNotValidException(NotificationMessageBase message) : base($"Notification message was not in a valid state.")
        {
            Data.Add("message", message);
        }
    }
}
