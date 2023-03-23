using System.Collections.Generic;
using Lykke.MarginTrading.Activities.Contracts.Models;

namespace Lykke.Snow.Notifications.Domain.Model.NotificationTypes
{
    public class AccountUnlockedNotification : NotificationMessage
    {
        public AccountUnlockedNotification(string title, string body, Dictionary<string, string> keyValueCollection) : base(title, body, keyValueCollection)
        {
        }
        
        public static AccountUnlockedNotification FromActivityEvent(ActivityEvent e)
        {
            var title = "Account unlocked";
            var body = "Trading and withdrawal is allowed.";

            return new AccountUnlockedNotification(title, body, new Dictionary<string, string>());
        }
    }
}
