using System.Collections.Generic;
using Lykke.MarginTrading.Activities.Contracts.Models;

namespace Lykke.Snow.Notifications.Domain.Model.NotificationTypes
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
