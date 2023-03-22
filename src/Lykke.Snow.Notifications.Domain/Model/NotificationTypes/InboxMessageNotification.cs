using System;
using System.Collections.Generic;
using Lykke.MarginTrading.Activities.Contracts.Models;

namespace Lykke.Snow.Notifications.Domain.Model.NotificationTypes
{
    //TODO: No activity - not implemented yet.
    public class InboxMessageNotification : NotificationMessage
    {
        public InboxMessageNotification(string title, string body, Dictionary<string, string> keyValueCollection) : base(title, body, keyValueCollection)
        {
            throw new NotImplementedException();
        }
        
        public static InboxMessageNotification FromActivityEvent(ActivityEvent e)
        {
            throw new NotImplementedException();
        }
    }
}
