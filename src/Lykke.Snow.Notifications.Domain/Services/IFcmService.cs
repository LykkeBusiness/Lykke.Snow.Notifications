using System.Threading.Tasks;
using FirebaseAdmin.Messaging;
using Lykke.Snow.Notifications.Domain.Model;

namespace Lykke.Snow.Notifications.Domain.Services
{
    public interface IFcmService
    {
        /// <summary>
        /// Creates the FirebaseApp Instance
        /// </summary>
        /// <param name="credentialsFilePath"></param>
        void CreateApp(string credentialsFilePath);

        /// <summary>
        /// Sends notification to the specified device
        /// </summary>
        /// <param name="message"></param>
        /// <param name="deviceToken"></param>
        /// <returns></returns>
        Task SendNotificationToDevice(NotificationMessage message, string deviceToken);
    }
}
