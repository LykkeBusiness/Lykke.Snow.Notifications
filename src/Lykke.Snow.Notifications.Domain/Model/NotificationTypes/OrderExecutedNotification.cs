using System.Collections.Generic;
using Lykke.MarginTrading.Activities.Contracts.Models;

namespace Lykke.Snow.Notifications.Domain.Model.NotificationTypes
{
    public class OrderExecutedNotification : NotificationMessage
    {
        public OrderExecutedNotification(string title, string body, Dictionary<string, string> keyValueCollection) : base(title, body, keyValueCollection)
        {
        }
        
        public static OrderExecutedNotification FromActivityEvent(ActivityEvent e)
        {
            return new OrderExecutedNotification("", "", new Dictionary<string, string>());
        }
    }
}
