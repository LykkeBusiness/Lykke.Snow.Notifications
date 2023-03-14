using System;
using System.Collections.Generic;

namespace Lykke.Snow.Notifications.Domain.Model
{
    /// <summary>
    /// Represents the base class for all notification types.
    /// </summary>
    public abstract class NotificationMessageBase
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
        public IDictionary<string, string> KeyValueBag { get; protected set; }
    }
}
