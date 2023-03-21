using System;
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
        public string Title { get; private set; }
        
        /// <summary>
        /// The notification payload
        /// </summary>
        /// <value></value>
        public string Body { get; private set; }
        
        /// <summary>
        /// Key-value dictionary for additional data to be sent along with the notification.
        /// </summary>
        /// <value></value>
        public IReadOnlyDictionary<string, string> KeyValueCollection { get; }

        protected NotificationMessage(string title, string body, Dictionary<string, string> keyValueCollection) 
        {
            if(string.IsNullOrEmpty(title))
                throw new ArgumentNullException(nameof(title));
            
            if(string.IsNullOrEmpty(body))
                throw new ArgumentNullException(nameof(body));
            
            Title = title;
            Body = body;
            KeyValueCollection = keyValueCollection;
        }
    }
}
