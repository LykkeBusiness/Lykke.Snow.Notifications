using System.Collections.Concurrent;
using System.Threading.Tasks;
using FirebaseAdmin.Messaging;
using Lykke.Snow.Common.Model;
using Lykke.Snow.FirebaseIntegration.Interfaces;

namespace Lykke.Snow.Notifications.Tests.Fakes
{
    public class FcmIntegrationServiceFake : IFcmIntegrationService
    {
        private static bool _isDeviceTokenValid = true;

        public ConcurrentBag<Message> ReceivedMessages { get; } = new ConcurrentBag<Message>();
        public static void SetIsDeviceTokenValid(bool value) => _isDeviceTokenValid = value;

        public Task<bool> IsDeviceTokenValid(string deviceToken)
        {
            return Task.FromResult(_isDeviceTokenValid);
        }

        public Task<Result<string, MessagingErrorCode>> SendNotification(Message message)
        {
            ReceivedMessages.Add(message);
            return new Result<string, MessagingErrorCode>(string.Empty);
        }
    }
}
