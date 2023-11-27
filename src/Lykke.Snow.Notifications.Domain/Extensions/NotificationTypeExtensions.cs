using System.Collections.Generic;
using Lykke.Snow.Notifications.Domain.Enums;

namespace Lykke.Snow.Notifications.Domain.Extensions
{
    public static class NotificationTypeExtensions
    {
        /// <summary>
        /// This function returns true if the given notification type is classified as 'OnBehalf' notificaiton.
        /// This segmentation is a part of business requirements regarding enabling/disabling all on-behalf notificaitons in bulk.
        /// </summary>
        /// <param name="notificationType"></param>
        /// <returns></returns>
        public static bool IsOnBehalfNotification(this NotificationType notificationType)
        {
            var onBehalfNotifications = new HashSet<NotificationType>
            {
                NotificationType.OnBehalfOrderPlacement,
                NotificationType.OnBehalfOrderModification,
                NotificationType.OnBehalfOrderCancellation,
                NotificationType.OnBehalfPositionClosing,
                NotificationType.OnBehalfOrderExecution
            };
            
            return onBehalfNotifications.Contains(notificationType);
        }
    }
}
