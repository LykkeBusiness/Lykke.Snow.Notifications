using System.Collections.Generic;
using Lykke.MarginTrading.Activities.Contracts.Models;

namespace Lykke.Snow.Notifications.Domain.Model.NotificationTypes
{
    public class DepositSucceededNotification : NotificationMessage
    {
        private DepositSucceededNotification(string title, string body, Dictionary<string, string> keyValueCollection) : base(title, body, keyValueCollection)
        {
        }

        public static DepositSucceededNotification FromActivityEvent(ActivityEvent e)
        {
            var amount = e.Activity.DescriptionAttributes[0];
            var currency = e.Activity.DescriptionAttributes[1];

            var title = "Deposit";
            var body = $"{currency}{amount} has been deposited to the account {e.Activity.AccountId}";

            return new DepositSucceededNotification(title, body, new Dictionary<string, string>());
        }
    }
}
