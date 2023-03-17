using System;
using FirebaseAdmin.Messaging;

namespace Lykke.Snow.FirebaseIntegration.Exceptions
{
    public sealed class CannotSendNotificationException : Exception
    {
        private const string ErrorMsg = "FCM could not deliver the message";

        public CannotSendNotificationException(
            Message fcmMessage, 
            MessagingErrorCode? fcmErrorCode, 
            Exception innerException) : base(ErrorMsg, innerException)
        {
            Data.Add(nameof(fcmMessage), fcmMessage);
            Data.Add(nameof(fcmErrorCode), fcmErrorCode);
        }
    }
}
