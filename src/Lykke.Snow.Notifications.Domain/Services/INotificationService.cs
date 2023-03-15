using System;
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
        /// Indicates that if the service is initialized
        /// </summary>
        /// <value></value>
        bool IsInitialized { get; }

        /// <summary>
        /// NotificationService must be initialized before sending any notification.
        /// This method initializes FirebaseApp, makes it ready to send notifications to Fcm Server
        /// </summary>
        /// <param name="credentialsFilePath">
        /// The path to the credentials file (in json format) in File system
        /// See https://firebase.google.com/docs/cloud-messaging/auth-server#provide-credentials-manually
        /// </param>
        void Initialize(string credentialsFilePath);

        /// <summary>
        /// Sends notification to a single device.
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        Task SendNotificationToSingleDevice(NotificationMessage message, string deviceToken);
    }
}
