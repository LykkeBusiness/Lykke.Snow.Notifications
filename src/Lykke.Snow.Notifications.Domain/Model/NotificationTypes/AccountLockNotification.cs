using System.Collections.Generic;
using Lykke.MarginTrading.Activities.Contracts.Models;

namespace Lykke.Snow.Notifications.Domain.Model.NotificationTypes
{
    public class AccountLockNotification : NotificationMessage
    {
        public AccountLockNotification(string title, string body, Dictionary<string, string> keyValueCollection) : base(title, body, keyValueCollection)
        {
        }
        
        public static AccountLockNotification FromActivityEvent(ActivityEvent e, bool locked)
        {
            // Account locked
            if(locked)
            {
                var title = "Account locked";
                var body = "Trading and withdrawal is not allowed.";

                return new AccountLockNotification(title, body, new Dictionary<string, string>());
            }

            // Account unlocked
            else
            {
                var title = "Account unlocked";
                var body = "Trading and withdrawal is allowed.";

                return new AccountLockNotification(title, body, new Dictionary<string, string>());
            }
        }
    }
}
