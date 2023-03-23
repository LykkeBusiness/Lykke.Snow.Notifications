using System.Collections.Generic;
using Lykke.MarginTrading.Activities.Contracts.Models;
using Lykke.Snow.Notifications.Domain.Model;

namespace Lykke.Snow.Notifications.Domain.Model.NotificationTypes
{
    public class WithdrawalFailedNotification : NotificationMessage
    {
        public WithdrawalFailedNotification(string title, string body, Dictionary<string, string> keyValueCollection) : base(title, body, keyValueCollection)
        {
        }
        
        public static WithdrawalFailedNotification FromActivityEvent(ActivityEvent e)
        {
            var amount = e.Activity.DescriptionAttributes[0];
            var currency = e.Activity.DescriptionAttributes[1];
            
            var title = "Withdrawal";
            var body = $"The withdrawal of {currency}{amount} to the account {e.Activity.AccountId} has failed.";

            return new WithdrawalFailedNotification(title, body, new Dictionary<string, string>());
        }
    }
}
