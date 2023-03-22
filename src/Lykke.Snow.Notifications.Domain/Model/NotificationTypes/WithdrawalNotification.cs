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
        
        public static WithdrawalNotification FromActivityEvent(ActivityEvent e, bool success)
        {
            var amount = e.Activity.DescriptionAttributes[0];
            var currency = e.Activity.DescriptionAttributes[1];
            
            
            // Withdrawal succeeded
            if(success)
            {
                var title = "Withdrawal";
                var body = $"The withdrawal of {currency}{amount} to the account {e.Activity.AccountId} has been completed successfully.";
                
                return new WithdrawalNotification(title, body, new Dictionary<string, string>());
            }
            
            // Withdrawal failed
            else
            {
                var title = "Withdrawal";
                var body = $"The withdrawal of {currency}{amount} to the account {e.Activity.AccountId} has failed.";

                return new WithdrawalNotification(title, body, new Dictionary<string, string>());
            }
        }
    }
}
