using System.Collections.Generic;
using Lykke.MarginTrading.Activities.Contracts.Models;

namespace Lykke.Snow.Notifications.Domain.Model.NotificationTypes
{
    public class DepositNotification : NotificationMessage
    {
        private DepositNotification(string title, string body, Dictionary<string, string> keyValueCollection) : base(title, body, keyValueCollection)
        {
        }

        public static DepositNotification FromActivityEvent(ActivityEvent e)
        {
            return new DepositNotification("", "", new Dictionary<string, string>());
        }
    }
}
