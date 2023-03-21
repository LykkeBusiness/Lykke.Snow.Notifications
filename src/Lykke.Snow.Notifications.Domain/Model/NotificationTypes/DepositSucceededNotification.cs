using System.Collections.Generic;
using Lykke.MarginTrading.Activities.Contracts.Models;
using Lykke.Snow.Notifications.Domain.Model;

namespace Lykke.Snow.Notifications.Domain.NotificationTypes
{
    public class DepositSucceededNotification : NotificationMessage
    {
        private DepositSucceededNotification(string title, string body, Dictionary<string, string> keyValueCollection) : base(title, body, keyValueCollection)
        {
        }

        public static DepositSucceededNotification FromActivityEvent(ActivityEvent activityEvent)
        {
            return new DepositSucceededNotification("", "", new Dictionary<string, string>());
        }
    }
}
