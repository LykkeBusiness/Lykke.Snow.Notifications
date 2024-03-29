using System;
using System.Collections.Generic;
using Lykke.Snow.Notifications.Domain.Enums;

namespace Lykke.Snow.Notifications.Domain.Model
{
    /// <summary>
    /// Represents the notification object
    /// </summary>
    public class NotificationMessage
    {
        /// <summary>
        /// Title for the notification
        /// </summary>
        /// <value></value>
        public string Title { get; }
        
        /// <summary>
        /// The notification payload
        /// </summary>
        /// <value></value>
        public string Body { get; }

        /// <summary>
        /// Notification Type (e.g. DepositSucceeded)
        /// </summary>
        /// <value></value>
        public NotificationType NotificationType { get; }
        
        /// <summary>
        /// Key-value dictionary for additional data to be sent along with the notification.
        /// </summary>
        /// <value></value>
        public IReadOnlyDictionary<string, string> KeyValueCollection { get; }

        public NotificationMessage(string title, string body, NotificationType type, Dictionary<string, string> keyValueCollection) 
        {
            if(string.IsNullOrEmpty(title))
                throw new ArgumentNullException(nameof(title));
            
            if(string.IsNullOrEmpty(body))
                throw new ArgumentNullException(nameof(body));
            
            Title = title;
            Body = body;
            KeyValueCollection = keyValueCollection;
            NotificationType = type;
        }
    }
}
