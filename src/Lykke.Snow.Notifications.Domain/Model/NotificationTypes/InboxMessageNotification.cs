using System.Collections.Generic;
using Lykke.MarginTrading.Activities.Contracts.Models;

namespace Lykke.Snow.Notifications.Domain.Model.NotificationTypes
{
    public class InboxMessageNotification : NotificationMessage
    {
        public InboxMessageNotification(string title, string body, Dictionary<string, string> keyValueCollection) : base(title, body, keyValueCollection)
        {
        }
        
        public static InboxMessageNotification FromActivityEvent(ActivityEvent e)
        {
            return new InboxMessageNotification("", "", new Dictionary<string, string>());
        }
    }
}
