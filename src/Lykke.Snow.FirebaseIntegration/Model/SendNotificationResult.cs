using FirebaseAdmin.Messaging;

namespace Lykke.Snow.FirebaseIntegration.Model
{
    /// <summary>
    /// Represents the result of operations that's been forwarded to Firebase Cloud Messaging server.
    /// </summary>
    public class SendNotificationResult
    {
        public bool IsSucceeded { get; private set; }
        public string FcmMessageId { get; private set; }
        public MessagingErrorCode? ErrorCode { get; private set; }
        
        private SendNotificationResult() {}
        
        public static SendNotificationResult Success(string messageId)
        {
            return new SendNotificationResult() { IsSucceeded = true, FcmMessageId = messageId };
        }

        public static SendNotificationResult Fail(MessagingErrorCode errorCode)
        {
            return new SendNotificationResult() { IsSucceeded = false, ErrorCode = errorCode };
        }
    }
}
