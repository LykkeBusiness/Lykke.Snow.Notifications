#nullable disable

using System;
using System.ComponentModel.DataAnnotations;
using JetBrains.Annotations;

namespace Lykke.Snow.Notifications.Client.Models
{
    /// <summary>
    /// Device configuration contract
    /// </summary>
    [PublicAPI]
    public class DeviceConfigurationContract
    {
        /// <summary>
        /// Device id
        /// </summary>
        [Required]
        public string DeviceId { get; set; } 
        
        /// <summary>
        /// Account id
        /// </summary>
        [Required]
        public string AccountId { get; set; }
        
        /// <summary>
        /// Locale for notifications
        /// </summary>
        [Required]
        public string Locale { get; set; }
        
        /// <summary>
        /// Notification types that are enabled
        /// </summary>
        public string[] NotificationsOn { get; set; }

        /// <summary>
        /// Notification types that are enabled
        /// </summary>
        public DateTime LastUpdated { get; set; }
    }
}
