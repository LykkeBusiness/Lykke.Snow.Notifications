using System.Threading.Tasks;
using FirebaseAdmin.Messaging;
using Lykke.Snow.Common.Model;

namespace Lykke.Snow.FirebaseIntegration.Interfaces
{
    /// <summary>
    /// Firebase Cloud Messaging client interface. Communicates to Firebase services.
    /// </summary>
    public interface IFcmIntegrationService
    {
        /// <summary>
        /// Sends notification to the specified device
        /// </summary>
        /// <param name="message"></param>
        /// <param name="deviceToken"></param>
        /// <returns></returns>
        Task<Result<string, MessagingErrorCode>> SendNotification(Message message, string deviceToken);
    }
}
