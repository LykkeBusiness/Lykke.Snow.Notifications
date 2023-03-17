using System.Collections.Generic;
using Lykke.Snow.Notifications.Domain.Model;

namespace Lykke.Snow.Notifications.Tests.Model
{
    public class DummyMessage : NotificationMessage
    {
        public DummyMessage(string title, string body, Dictionary<string, string> keyValueBag) : base(title, body, keyValueBag)
        {
        }
    }
}
