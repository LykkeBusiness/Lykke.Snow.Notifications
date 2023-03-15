using System.Threading.Tasks;
using Lykke.Snow.Notifications.Domain.Model;

namespace Lykke.Snow.Notifications.Domain.Services
{
    /// <summary>
    /// Firebase Cloud Messaging client interface. Communicates to Firebase services.
    /// </summary>
    public interface IFcmService
    {
        /// <summary>
        /// Creates the FirebaseApp Instance
        /// </summary>
        void CreateApp();

        /// <summary>
        /// Sends notification to the specified device
        /// </summary>
        /// <param name="message"></param>
        /// <param name="deviceToken"></param>
        /// <returns></returns>
        Task SendNotificationToDevice(NotificationMessage message, string deviceToken);
    }
}
