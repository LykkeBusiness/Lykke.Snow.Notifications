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
            return new MarginCall1Notification("", "", new Dictionary<string, string>());
        }
    }
}
