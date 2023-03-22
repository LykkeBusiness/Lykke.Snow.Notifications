using System.Collections.Generic;
using Lykke.MarginTrading.Activities.Contracts.Models;
using Lykke.Snow.Notifications.Domain.Model;

namespace Lykke.Snow.Notifications.Domain.NotificationTypes
{
    public class WithdrawalNotification : NotificationMessage
    {
        public WithdrawalNotification(string title, string body, Dictionary<string, string> keyValueCollection) : base(title, body, keyValueCollection)
        {
        }
        
        public static WithdrawalNotification FromActivityEvent(ActivityEvent e)
        {
            return new WithdrawalNotification("", "", new Dictionary<string, string>());
        }
    }
}
