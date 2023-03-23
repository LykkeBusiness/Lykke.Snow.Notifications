using System.Collections.Generic;
using Lykke.MarginTrading.Activities.Contracts.Models;

namespace Lykke.Snow.Notifications.Domain.Model.NotificationTypes
{
    public class AccountLockedNotification : NotificationMessage
    {
        public AccountLockedNotification(string title, string body, Dictionary<string, string> keyValueCollection) : base(title, body, keyValueCollection)
        {
        }
        
        public static AccountLockedNotification FromActivityEvent(ActivityEvent e)
        {
            var title = "Account locked";
            var body = "Trading and withdrawal is not allowed.";

            return new AccountLockedNotification(title, body, new Dictionary<string, string>());
        }
    }
}
