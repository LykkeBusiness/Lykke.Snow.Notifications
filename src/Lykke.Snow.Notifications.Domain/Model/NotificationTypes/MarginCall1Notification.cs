using System.Collections.Generic;
using Lykke.MarginTrading.Activities.Contracts.Models;

namespace Lykke.Snow.Notifications.Domain.Model.NotificationTypes
{
    public class MarginCall1Notification : NotificationMessage
    {
        public MarginCall1Notification(string title, string body, Dictionary<string, string> keyValueCollection) : base(title, body, keyValueCollection)
        {
        }
        
        public static MarginCall1Notification FromActivityEvent(ActivityEvent e)
        {
            //TODO: find out the actual title / body
            return new MarginCall1Notification("Margin call 1", "Margin call 1 body", new Dictionary<string, string>());
        }
    }
}
