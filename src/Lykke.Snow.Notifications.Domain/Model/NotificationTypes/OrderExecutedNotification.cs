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
            var direction = e.Activity.DescriptionAttributes[0];
            var orderType = e.Activity.DescriptionAttributes[1];
            var quantity = e.Activity.DescriptionAttributes[2];
            var productName = e.Activity.DescriptionAttributes[3];
            
            var title = "Order executed";
            var body = $"{direction} {orderType} order for {quantity} {productName} has been executed";

            return new OrderExecutedNotification(title, body, new Dictionary<string, string>());
        }
    }
}
