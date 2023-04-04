namespace Lykke.Snow.Notifications.Client.Models
{
    /// <summary>
    /// Device configuration error codes
    /// </summary>
    public enum DeviceConfigurationErrorCodeContract
    {
        /// <summary>
        /// No error, operation was successful
        /// </summary>
        None,
        
        /// <summary>
        /// Configuration for device does not exist
        /// </summary>
        DoesNotExist,
        
        /// <summary>
        /// Device id or account id or notification type are empty
        /// </summary>
        InvalidInput,
        
        /// <summary>
        /// Locale is not supported
        /// </summary>
        UnsupportedLocale,
        
        /// <summary>
        /// Notification type is not supported
        /// </summary>
        UnsupportedNotificationType,
        
        /// <summary>
        /// The list of notification types contains duplicates
        /// </summary>
        DuplicateNotificationType,
        
        /// <summary>
        /// There is a conflict when trying to add new configuration whereas it
        /// already exists or update existing configuration whereas it does not exist
        /// </summary>
        Conflict,
    }
}
