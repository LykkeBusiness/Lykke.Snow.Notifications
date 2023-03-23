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
            var direction = e.Activity.DescriptionAttributes[0];
            var orderType = e.Activity.DescriptionAttributes[1];
            var quantity = e.Activity.DescriptionAttributes[2];
            var productName = e.Activity.DescriptionAttributes[3];

            var title = "Order expired";
            var body = $"{direction} {orderType} order for {quantity} {productName} expired.";

            return new OrderExpiredNotification(title, body, new Dictionary<string, string>());
        }
    }
}
