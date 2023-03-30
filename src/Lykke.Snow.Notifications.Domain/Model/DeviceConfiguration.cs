using System;
using System.Collections.Generic;
using System.Linq;
using Lykke.Snow.Notifications.Domain.Enums;

namespace Lykke.Snow.Notifications.Domain.Model
{
    /// <summary>
    /// Device configuration
    /// </summary>
    public sealed class DeviceConfiguration
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

            public override string ToString()
            {
                return $"{Type} - {Enabled}";
            }

            public NotificationType Type { get; }
            public bool Enabled { get; }
        }
        public string DeviceId { get; }
        public string AccountId { get; }
        public Locale Locale { get; }
        public IReadOnlyList<Notification> Notifications { get; }

        /// <summary>
        /// The list of enabled notification types. Potentially, can be replaced
        /// with the list of enabled notifications if and when we have additional
        /// properties for a notification 
        /// </summary>
        public HashSet<NotificationType> EnabledNotificationTypes =>
            Notifications.Where(n => n.Enabled).Select(n => n.Type).ToHashSet();

        /// <summary>
        /// Returns true if notification type is enabled
        /// </summary>
        /// <param name="type">Notification type enum</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public bool IsNotificationEnabled(NotificationType type)
        {
            if (Enum.IsDefined(typeof(NotificationType), type))
                return EnabledNotificationTypes.Contains(type);

            throw new ArgumentException($"Notification type [{type}] is not supported");
        }

        /// <summary>
        /// Returns true if notification type is enabled
        /// </summary>
        /// <param name="type">Notification type string</param>
        /// <returns></returns>
        public bool IsNotificationEnabled(string type) =>
            IsNotificationEnabled(Enum.Parse<NotificationType>(type, true));

        /// <summary>
        /// Creates new device configuration
        /// </summary>
        /// <param name="deviceId">Device id</param>
        /// <param name="accountId">Account id</param>
        /// <param name="locale">Supported locale</param>
        /// <param name="notifications">Notification list</param>
        /// <exception cref="ArgumentNullException">When device id is empty or account id is empty</exception>
        /// <exception cref="ArgumentException">When locale is not supported</exception>
        public DeviceConfiguration(string deviceId,
            string accountId,
            string? locale = "en",
            IReadOnlyList<Notification>? notifications = null)
        {
            if (string.IsNullOrWhiteSpace(deviceId))
                throw new ArgumentNullException(nameof(deviceId), "Device id cannot be null or empty");

            if (string.IsNullOrWhiteSpace(accountId))
                throw new ArgumentNullException(nameof(accountId), "Account id cannot be null or empty");

            if (!Enum.TryParse<Locale>(locale, true, out var localeParsed))
                throw new ArgumentException($"Locale type [{locale}] is not supported");

            var duplicatedNotifications = notifications?
                .GroupBy(n => n.Type)
                .Where(g => g.Count() > 1)
                .ToList();
            if (duplicatedNotifications != null && duplicatedNotifications.Any())
                throw new ArgumentException(
                    $"Duplicated notifications: {string.Join(", ", duplicatedNotifications.Select(g => g.Key))}");
            
            DeviceId = deviceId;
            AccountId = accountId;
            Locale = localeParsed;
            Notifications = notifications ?? new List<Notification>();
        }

        public override string ToString()
        {
            return $"{nameof(DeviceId)}: {DeviceId}, " +
                   $"{nameof(AccountId)}: {AccountId}, " +
                   $"{nameof(Locale)}: {Locale}, " +
                   $"{nameof(Notifications)}: " +
                   $"{string.Join(", ", Notifications)}";
        }

        /// <summary>
        /// Creates default device configuration with all notification types enabled
        /// </summary>
        /// <param name="deviceId">Device id</param>
        /// <param name="accountId">Account id</param>
        /// <param name="locale">Locale</param>
        /// <returns></returns>
        public static DeviceConfiguration Default(string deviceId, string accountId, string locale = "en")
        {
            var allNotificationTypes = Enum.GetValues(typeof(NotificationType))
                .Cast<NotificationType>()
                .Select(t => new Notification(t.ToString()));

            return new DeviceConfiguration(deviceId, 
                accountId, 
                locale: locale,
                notifications: allNotificationTypes.ToList());
        }
    }
}
