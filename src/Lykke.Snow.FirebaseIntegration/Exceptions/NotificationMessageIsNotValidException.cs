using System;
using FirebaseAdmin.Messaging;

namespace Lykke.Snow.FirebaseIntegration.Exceptions
{
    public class NotificationMessageIsNotValidException : Exception
    {
        private const string ErrorMsg = "Notification message was not in a valid state.";

        public NotificationMessageIsNotValidException(
            Message fcmMessage, 
            Exception innerException, 
            string? additionalInfo = null) : base(ErrorMsg)
        {
            Data.Add(nameof(fcmMessage), fcmMessage);
            
            if(additionalInfo != null)
                Data.Add(nameof(additionalInfo), additionalInfo);
        }
    }
}
