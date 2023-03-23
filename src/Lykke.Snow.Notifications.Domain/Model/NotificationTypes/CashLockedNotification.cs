using System.Collections.Generic;
using Lykke.MarginTrading.Activities.Contracts.Models;

namespace Lykke.Snow.Notifications.Domain.Model.NotificationTypes
{
    public class CashLockedNotification : NotificationMessage
    {
        public CashLockedNotification(string title, string body, Dictionary<string, string> keyValueCollection) : base(title, body, keyValueCollection)
        {
        }
        
        public static CashLockedNotification FromActivityEvent(ActivityEvent e)
        {
            var title = "Cash locked";
            var body = "Withdrawal is not allowed.";
            
            return new CashLockedNotification(title, body, new Dictionary<string, string>());
        }
    }
}
