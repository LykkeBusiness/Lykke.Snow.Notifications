using Lykke.Snow.Notifications.Domain.Model;

namespace Lykke.Snow.Notifications.Tests.Model
{
    public class DummyNotificationType : NotificationMessage
    {
        public DummyNotificationType(string title, string body) : base(title, body)
        {
        }
    }
}
