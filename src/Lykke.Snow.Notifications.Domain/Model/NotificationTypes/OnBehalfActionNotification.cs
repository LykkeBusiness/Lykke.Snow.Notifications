using System;
using System.Collections.Generic;
using Lykke.MarginTrading.Activities.Contracts.Models;

namespace Lykke.Snow.Notifications.Domain.Model.NotificationTypes
{
    //TODO: No activity - not implemented yet.
    public class OnBehalfActionNotification : NotificationMessage
    {
        public OnBehalfActionNotification(string title, string body, Dictionary<string, string> keyValueCollection) : base(title, body, keyValueCollection)
        {
            throw new NotImplementedException();
        }
        
        public static OnBehalfActionNotification FromActivityEvent(ActivityEvent e)
        {
            throw new NotImplementedException();
        }
    }
}
