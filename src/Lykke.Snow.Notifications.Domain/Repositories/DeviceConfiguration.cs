﻿using System;
using System.Collections.Generic;

namespace Lykke.Snow.Notifications.Domain.Repositories
{
    public class DeviceConfiguration
    {
        public class Notification
        {
            public Notification(string type, bool enabled = true)
            {
                if (string.IsNullOrEmpty(type))
                    throw new ArgumentNullException(nameof(type), "Notification type cannot be null or empty");
                
                // todo: check for allowed types
                
                Type = type;
                Enabled = enabled;
            }

            public string Type { get; }
            public bool Enabled { get; }
        }
        public string DeviceId { get; }
        public string AccountId { get; }
        
        public string Locale { get; set; }
        public IReadOnlyList<Notification> Notifications { get; }
        
        public DeviceConfiguration(string deviceId, string accountId, string? locale = "en", IReadOnlyList<Notification>? notifications = null)
        {
            if (string.IsNullOrEmpty(deviceId))
                throw new ArgumentNullException(nameof(deviceId), "Device id cannot be null or empty");
            
            // todo: if we gonna have specific deviceId format then we'll probably need separate value object for it
            
            if (string.IsNullOrEmpty(accountId))
                throw new ArgumentNullException(nameof(accountId), "Account id cannot be null or empty");
            
            // todo: check the locale to ne in the list of allowed locales
            if (string.IsNullOrEmpty(locale))
                throw new ArgumentNullException(nameof(locale), "Locale cannot be null or empty");
            
            DeviceId = deviceId;
            AccountId = accountId;
            Locale = locale;
            Notifications = notifications ?? new List<Notification>();
        }
    }
}