using System;

namespace Lykke.Snow.Notifications.Domain.Exceptions
{
    public class NotificationServiceNotInitializedException : Exception
    {
        private const string ErrorMsg = "Notification service has not been initialized yet.";

        public NotificationServiceNotInitializedException() : base(ErrorMsg)
        {
        }
    }
}
