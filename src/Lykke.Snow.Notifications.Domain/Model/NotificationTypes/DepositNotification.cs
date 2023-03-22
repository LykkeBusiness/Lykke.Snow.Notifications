using System.Collections.Generic;
using Lykke.MarginTrading.Activities.Contracts.Models;

namespace Lykke.Snow.Notifications.Domain.Model.NotificationTypes
{
    public class DepositNotification : NotificationMessage
    {
        private DepositNotification(string title, string body, Dictionary<string, string> keyValueCollection) : base(title, body, keyValueCollection)
        {
        }

        public static DepositNotification FromActivityEvent(ActivityEvent e, bool success)
        {
            var amount = e.Activity.DescriptionAttributes[0];
            var currency = e.Activity.DescriptionAttributes[1];
            
            // Deposit succeeded
            if(success)
            {
                var title = "Deposit";
                var body = $"{currency}{amount} has been deposited to the account {e.Activity.AccountId}";

                return new DepositNotification(title, body, new Dictionary<string, string>());
            }
            //Deposit failed
            else
            {
                var title = "Deposit";
                var body = $"The deposit of {currency}{amount} to the account {e.Activity.AccountId} has failed.";

                return new DepositNotification(title, body, new Dictionary<string, string>());
            }
        }
    }
}
