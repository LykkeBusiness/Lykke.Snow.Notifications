using System.Collections.Generic;
using Lykke.MarginTrading.Activities.Contracts.Models;

namespace Lykke.Snow.Notifications.Domain.Model
{
    /// <summary>
    /// Represents the base class for all notification types. 
    /// </summary>
    public abstract class NotificationMessage
    {
        /// <summary>
        /// Title for the notification
        /// </summary>
        /// <value></value>
        public string Title { get; protected set; }
        
        /// <summary>
        /// The notification payload
        /// </summary>
        /// <value></value>
        public string Body { get; protected set; }
        
        /// <summary>
        /// Key-value dictionary for additional data to be sent along with the notification.
        /// </summary>
        /// <value></value>
        public Dictionary<string, string> KeyValueBag { get; protected set; }

        protected NotificationMessage(string title, string body, Dictionary<string, string> keyValueBag = null) 
        {
            Title = title;
            Body = body;
            KeyValueBag = keyValueBag;
        }
    }
}
