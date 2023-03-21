using System.Collections.Generic;
using Lykke.MarginTrading.Activities.Contracts.Models;

namespace Lykke.Snow.Notifications.Domain.Model.NotificationTypes
{
    public class DepositFailedNotification : NotificationMessage
    {
        public DepositFailedNotification(string title, string body, Dictionary<string, string> keyValueCollection) : base(title, body, keyValueCollection)
        {
        }
        
        public static DepositFailedNotification FromActivityEvent(ActivityEvent e)
        {
            return new DepositFailedNotification("", "", new Dictionary<string, string>());
        }
    }
}
