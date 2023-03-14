using System;
using Lykke.Snow.Notifications.Domain.Services;

namespace Lykke.Snow.Notifications.Domain.Exceptions
{
    public class NotificationServiceNotInitializedException : Exception
    {
        public NotificationServiceNotInitializedException() : base(@$"Notification service has not been initialized yet. 
            Method {nameof(INotificationService.Initialize)} must be called first.")
        {
        }
    }
}
