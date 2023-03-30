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
        /// Builds notification message based on given notification type and language
        /// </summary>
        /// <param name="notificationType"></param>
        /// <param name="args"></param>
        /// <param name="locale"></param>
        /// <returns></returns>
        Task<NotificationMessage> BuildLocalizedNotificationMessage(NotificationType notificationType, string[] args, string? locale, Dictionary<string, string> keyValuePairs);

        /// <summary>
        /// Builds notification type with given title and body
        /// </summary>
        /// <param name="notificationType"></param>
        /// <param name="args"></param>
        /// <param name="locale"></param>
        /// <returns></returns>
        NotificationMessage BuildNotificationMessage(NotificationType notificationType, string title, string body, Dictionary<string, string> keyValuePairs);

        /// <summary>
        /// Checks if the notification type is enabled for the given device
        /// </summary>
        /// <param name="notificationType"></param>
        /// <param name="args"></param>
        /// <param name="locale"></param>
        /// <returns></returns>
        bool IsDeviceTargeted(DeviceConfiguration deviceConfiguration, NotificationType type);
    }
}
