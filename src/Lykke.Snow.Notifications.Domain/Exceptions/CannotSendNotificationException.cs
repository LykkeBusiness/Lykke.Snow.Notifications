using System;
using FirebaseAdmin.Messaging;
using Lykke.Snow.Notifications.Domain.Model;

namespace Lykke.Snow.Notifications.Domain.Exceptions
{
    public class CannotSendNotificationException : Exception
    {
        private readonly MessagingErrorCode _fcmErrorCode;
        public CannotSendNotificationException(NotificationMessage notificationMessage, Message fcmMessage, Exception innerException) 
            : base("FCM could not deliver the message.", innerException)
        {
            Data.Add(nameof(notificationMessage), notificationMessage);
            Data.Add(nameof(fcmMessage), fcmMessage);
        }
    }
}
