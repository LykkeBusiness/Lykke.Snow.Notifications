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
            return new PositionClosedNotification("", "", new Dictionary<string, string>());
        }
    }
}
