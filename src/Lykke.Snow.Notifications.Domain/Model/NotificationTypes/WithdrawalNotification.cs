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
            var amount = e.Activity.DescriptionAttributes[0];
            var currency = e.Activity.DescriptionAttributes[1];
            
            var body = $"The withdrawal operation ({currency}{amount}) has been successfully completed.";

            return new WithdrawalNotification(title: "Withdrawal", 
                body: body, new Dictionary<string, string>());
        }
    }
}
