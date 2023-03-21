using System.Collections.Generic;
using Lykke.MarginTrading.Activities.Contracts.Models;

namespace Lykke.Snow.Notifications.Domain.Model.NotificationTypes
{
    public class OnBehalfActionNotification : NotificationMessage
    {
        public OnBehalfActionNotification(string title, string body, Dictionary<string, string> keyValueCollection) : base(title, body, keyValueCollection)
        {
        }
        
        public static OnBehalfActionNotification FromActivityEvent(ActivityEvent e)
        {
            return new OnBehalfActionNotification("", "", new Dictionary<string, string>());
        }
    }
}
