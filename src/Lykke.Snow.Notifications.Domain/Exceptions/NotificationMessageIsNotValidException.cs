using System;
using FirebaseAdmin.Messaging;
using Lykke.Snow.Notifications.Domain.Model;

namespace Lykke.Snow.Notifications.Domain.Exceptions
{
    public class NotificationMessageIsNotValidException : Exception
    {
        private const string ErrorMsg = "Notification message was not in a valid state.";

        public NotificationMessageIsNotValidException(
            NotificationMessage notificationMessage, 
            Message fcmMessage, 
            Exception innerException, 
            string additionalInfo = null) : base(ErrorMsg)
        {
            Data.Add(nameof(notificationMessage), notificationMessage);
            Data.Add(nameof(fcmMessage), fcmMessage);
            
            if(additionalInfo != null)
                Data.Add(nameof(additionalInfo), additionalInfo);
        }
    }
}
