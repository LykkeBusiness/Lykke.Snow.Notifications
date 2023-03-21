using System.Collections.Generic;
using Lykke.MarginTrading.Activities.Contracts.Models;

namespace Lykke.Snow.Notifications.Domain.Model.NotificationTypes
{
    public class AccountLockNotification : NotificationMessage
    {
        public AccountLockNotification(string title, string body, Dictionary<string, string> keyValueCollection) : base(title, body, keyValueCollection)
        {
        }
        
        public static AccountLockNotification FromActivityEvent(ActivityEvent e)
        {
            return new AccountLockNotification("", "", new Dictionary<string, string>());
        }
    }
}
