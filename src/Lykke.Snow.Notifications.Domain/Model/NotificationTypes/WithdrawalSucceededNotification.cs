using System.Collections.Generic;
using Lykke.MarginTrading.Activities.Contracts.Models;
using Lykke.Snow.Notifications.Domain.Model;

namespace Lykke.Snow.Notifications.Domain.NotificationTypes
{
    public class WithdrawalSucceededNotification : NotificationMessage
    {
        public WithdrawalSucceededNotification(string title, string body, Dictionary<string, string> keyValueCollection) : base(title, body, keyValueCollection)
        {
        }
        
        public static WithdrawalSucceededNotification FromActivityEvent(ActivityEvent e)
        {
            return new WithdrawalSucceededNotification("", "", new Dictionary<string, string>());
        }
    }
}
