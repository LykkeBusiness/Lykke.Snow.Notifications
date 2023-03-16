using System.Threading.Tasks;
using FirebaseAdmin.Messaging;
using Lykke.Snow.FirebaseIntegration.Model;

namespace Lykke.Snow.FirebaseIntegration.Interfaces
{
    /// <summary>
    /// Firebase Cloud Messaging client interface. Communicates to Firebase services.
    /// </summary>
    public interface IFcmIntegrationService
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
        Task<SendNotificationResult> SendNotification(Message message, string deviceToken);
    }
}
