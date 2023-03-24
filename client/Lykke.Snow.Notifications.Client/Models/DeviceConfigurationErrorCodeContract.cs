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
        /// The input violates business rules
        /// </summary>
        InvalidInput,
        
        /// <summary>
        /// There is a conflict when trying to add new configuration whereas it
        /// already exists or update existing configuration whereas it does not exist
        /// </summary>
        Conflict,
    }
}
