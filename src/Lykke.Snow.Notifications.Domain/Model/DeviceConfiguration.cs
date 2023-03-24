using System;
using System.Collections.Generic;
using System.Linq;
using Lykke.Snow.Notifications.Domain.Model;

namespace Lykke.Snow.Notifications.Domain.Model
{
    // todo: insert configuration upong adding new token registration if it doesn't exist

    /// <summary>
    /// Device configuration
    /// </summary>
    public class DeviceConfiguration
    {
        public class Notification
        {
            public Notification(string type, bool enabled = true)
            {
                if (string.IsNullOrWhiteSpace(type))
                    throw new ArgumentNullException(nameof(type), "Notification type cannot be null or empty");

                if (!Enum.TryParse<NotificationType>(type, true, out var notificationType))
                    throw new ArgumentException($"Notification type [{type}] is not supported");

                Type = notificationType;
                Enabled = enabled;
            }

            public NotificationType Type { get; }
            public bool Enabled { get; }
        }

        public string DeviceId { get; }
        public string AccountId { get; }
        public string Locale { get; set; }
        public IReadOnlyList<Notification> Notifications { get; }

        public IEnumerable<Notification> EnabledNotifications => Notifications.Where(n => n.Enabled);

        public bool IsNotificationEnabled(NotificationType type)
        {
            if (Enum.IsDefined(typeof(NotificationType), type))
                return EnabledNotifications.Any(n => n.Type == type);
            
            throw new ArgumentException($"Notification type [{type}] is not supported");
        }

        public bool IsNotificationEnabled(string type) =>
            IsNotificationEnabled(Enum.Parse<NotificationType>(type, true));

        public DeviceConfiguration(string deviceId,
            string accountId,
            string? locale = "en",
            IReadOnlyList<Notification>? notifications = null)
        {
            if (string.IsNullOrWhiteSpace(deviceId))
                throw new ArgumentNullException(nameof(deviceId), "Device id cannot be null or empty");

            // todo: if we gonna have specific deviceId format then we'll probably need separate value object for it

            if (string.IsNullOrWhiteSpace(accountId))
                throw new ArgumentNullException(nameof(accountId), "Account id cannot be null or empty");

            // todo: check the locale to ne in the list of allowed locales
            if (string.IsNullOrWhiteSpace(locale))
                throw new ArgumentNullException(nameof(locale), "Locale cannot be null or empty");

            DeviceId = deviceId;
            AccountId = accountId;
            Locale = locale;
            Notifications = notifications ?? new List<Notification>();
        }

        /// <summary>
        /// Creates default device configuration with all notification types enabled
        /// </summary>
        /// <param name="deviceId">Device id</param>
        /// <param name="accountId">Account id</param>
        /// <returns></returns>
        public static DeviceConfiguration Default(string deviceId, string accountId)
        {
            var allNotificationTypes = Enum.GetValues(typeof(NotificationType))
                .Cast<NotificationType>()
                .Select(t => new Notification(t.ToString()));
            
            return new DeviceConfiguration(deviceId, accountId, notifications: allNotificationTypes.ToList());
        }
    }
}
