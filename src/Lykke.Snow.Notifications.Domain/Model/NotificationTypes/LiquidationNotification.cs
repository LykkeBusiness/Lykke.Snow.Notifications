using System.Collections.Generic;
using Lykke.MarginTrading.Activities.Contracts.Models;

namespace Lykke.Snow.Notifications.Domain.Model.NotificationTypes
{
    public class LiquidationNotification : NotificationMessage
    {
        public LiquidationNotification(string title, string body, Dictionary<string, string> keyValueCollection) : base(title, body, keyValueCollection)
        {
        }

        public static LiquidationNotification FromActivityEvent(ActivityEvent e)
        {
            return new LiquidationNotification("", "", new Dictionary<string, string>());
        }
    }
}
