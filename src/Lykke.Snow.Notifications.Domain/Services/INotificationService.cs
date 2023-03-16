using System.Threading.Tasks;
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
        /// <returns></returns>
        Task SendNotification(NotificationMessage message, string deviceToken);
    }
}
