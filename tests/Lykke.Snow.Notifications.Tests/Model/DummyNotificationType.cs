using System.Collections.Generic;
using Lykke.Snow.Notifications.Domain.Model;

namespace Lykke.Snow.Notifications.Tests.Model
{
    public class DummyNotificationType : NotificationMessage
    {
        public DummyNotificationType(string title, string body, Dictionary<string, string> keyValueBag = null) : base(title, body, keyValueBag)
        {
        }
    }
}
