using System.Collections.Generic;
using Lykke.MarginTrading.Activities.Contracts.Models;

namespace Lykke.Snow.Notifications.Domain.Model.NotificationTypes
{
    public class CashLockNotification : NotificationMessage
    {
        public CashLockNotification(string title, string body, Dictionary<string, string> keyValueCollection) : base(title, body, keyValueCollection)
        {
        }
        
        public static CashLockNotification FromActivityEvent(ActivityEvent e)
        {
            return new CashLockNotification("", "", new Dictionary<string, string>());
        }
    }
}
