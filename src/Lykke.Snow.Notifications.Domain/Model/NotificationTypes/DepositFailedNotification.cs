using System.Collections.Generic;
using Lykke.MarginTrading.Activities.Contracts.Models;

namespace Lykke.Snow.Notifications.Domain.Model.NotificationTypes
{
    public class DepositFailedNotification : NotificationMessage
    {
        private DepositFailedNotification(string title, string body, Dictionary<string, string> keyValueCollection) : base(title, body, keyValueCollection)
        {
        }

        public static DepositFailedNotification FromActivityEvent(ActivityEvent e)
        {
            var amount = e.Activity.DescriptionAttributes[0];
            var currency = e.Activity.DescriptionAttributes[1];

            var title = "Deposit";
            var body = $"The deposit of {currency}{amount} to the account {e.Activity.AccountId} has failed.";

            return new DepositFailedNotification(title, body, new Dictionary<string, string>());
        }
    }
}
