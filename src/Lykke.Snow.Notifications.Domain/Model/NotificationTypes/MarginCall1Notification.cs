using System.Collections.Generic;
using Lykke.MarginTrading.Activities.Contracts.Models;
using Lykke.Snow.Notifications.Domain.Model;

namespace Lykke.Snow.Notifications.Domain.NotificationTypes
{
    public class MarginCall2Notification : NotificationMessage
    {
        public MarginCall2Notification(string title, string body, Dictionary<string, string> keyValueCollection) : base(title, body, keyValueCollection)
        {
        }
        
        public static MarginCall2Notification FromActivityEvent(ActivityEvent e)
        {
            return new MarginCall2Notification("", "", new Dictionary<string, string>());
        }
    }
}
