using System.Collections.Generic;
using Lykke.MarginTrading.Activities.Contracts.Models;

namespace Lykke.Snow.Notifications.Domain.Model.NotificationTypes
{
    public class CashUnlockedNotification : NotificationMessage
    {
        public CashUnlockedNotification(string title, string body, Dictionary<string, string> keyValueCollection) : base(title, body, keyValueCollection)
        {
        }
        
        public static CashUnlockedNotification FromActivityEvent(ActivityEvent e)
        {
            var title = "Cash unlocked";
            var body = "Withdrawal is allowed.";
            
            return new CashUnlockedNotification(title, body, new Dictionary<string, string>());
        }
    }
}
