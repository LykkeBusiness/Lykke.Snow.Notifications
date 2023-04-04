using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Snow.Notifications.Domain.Enums;
using Lykke.Snow.Notifications.Domain.Model;

namespace Lykke.Snow.Notifications.Domain.Services
{
    /// <summary>
    /// Handles notification concerns at application level.
    /// </summary>
    public interface INotificationService
    {
        /// <summary>
        /// Sends notification to a single device.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="deviceToken"></param>
        /// <returns></returns>
        Task SendNotification(NotificationMessage message, string deviceToken);

        /// <summary>
        /// Builds notification type with given title and body
        /// </summary>
        /// <param name="notificationType"></param>
        /// <param name="title"></param>
        /// <param name="body"></param>
        /// <param name="keyValuePairs"></param>
        /// <returns></returns>
        NotificationMessage BuildNotificationMessage(NotificationType notificationType,
            string? title,
            string? body,
            Dictionary<string, string> keyValuePairs);

        /// <summary>
        /// Checks if the notification type is enabled for the given device
        /// </summary>
        /// <param name="deviceConfiguration"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        bool IsDeviceTargeted(DeviceConfiguration deviceConfiguration, NotificationType type);
    }
}
