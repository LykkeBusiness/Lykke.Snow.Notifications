using System.Collections.Generic;
using Lykke.MarginTrading.Activities.Contracts.Models;

namespace Lykke.Snow.Notifications.Domain.Model.NotificationTypes
{
    public class PositionClosedNotification : NotificationMessage
    {
        public PositionClosedNotification(string title, string body, Dictionary<string, string> keyValueCollection) : base(title, body, keyValueCollection)
        {
        }
        
        public static PositionClosedNotification FromActivityEvent(ActivityEvent e)
        {
            var direction = e.Activity.DescriptionAttributes[0];
            var quantity = e.Activity.DescriptionAttributes[1];
            var productName = e.Activity.DescriptionAttributes[2];
            var productLoss = e.Activity.DescriptionAttributes[3];
            var sc = e.Activity.DescriptionAttributes[4];

            var title = "Position closed";
            var body = "";

            return new PositionClosedNotification("", "", new Dictionary<string, string>());
        }
    }
}
