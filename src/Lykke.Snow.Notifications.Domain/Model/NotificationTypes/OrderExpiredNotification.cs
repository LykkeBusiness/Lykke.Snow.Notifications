using System.Collections.Generic;
using Lykke.MarginTrading.Activities.Contracts.Models;

namespace Lykke.Snow.Notifications.Domain.Model.NotificationTypes
{
    public class OrderExpiredNotification : NotificationMessage
    {
        public OrderExpiredNotification(string title, string body, Dictionary<string, string> keyValueCollection) : base(title, body, keyValueCollection)
        {
        }
        
        public static OrderExpiredNotification FromActivityEvent(ActivityEvent e)
        {
            return new OrderExpiredNotification("", "", new Dictionary<string, string>());
        }
    }
}
