using System;
using FirebaseAdmin.Messaging;
using Lykke.Snow.Notifications.Domain.Model;

namespace Lykke.Snow.Notifications.Domain.Exceptions
{
    public class NotificationMessageIsNotValidException : Exception
    {
        public NotificationMessageIsNotValidException(NotificationMessage notificationMessage, Message fcmMessage, Exception innerException, string additionalInfo = null) : base($"Notification message was not in a valid state.")
        {
            Data.Add(nameof(notificationMessage), notificationMessage);
            Data.Add(nameof(fcmMessage), fcmMessage);
            
            if(additionalInfo != null)
                Data.Add(nameof(additionalInfo), additionalInfo);
        }
    }
}
