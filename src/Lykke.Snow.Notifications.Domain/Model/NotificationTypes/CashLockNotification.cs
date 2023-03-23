using System.Collections.Generic;
using Lykke.MarginTrading.Activities.Contracts.Models;

namespace Lykke.Snow.Notifications.Domain.Model.NotificationTypes
{
    public class CashLockNotification : NotificationMessage
    {
        public CashLockNotification(string title, string body, Dictionary<string, string> keyValueCollection) : base(title, body, keyValueCollection)
        {
        }
        
        public static CashLockNotification FromActivityEvent(ActivityEvent e, bool locked)
        {
            // Cash locked
            if(locked)
            {
                var title = "Cash locked";
                var body = "Withdrawal is not allowed.";
                
                return new CashLockNotification(title, body, new Dictionary<string, string>());
            }
            
            // cash unlocked
            else
            {
                var title = "Cash unlocked";
                var body = "Withdrawal is allowed.";
                
                return new CashLockNotification(title, body, new Dictionary<string, string>());
            }
        }
    }
}
