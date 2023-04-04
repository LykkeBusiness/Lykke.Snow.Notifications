namespace Lykke.Snow.Notifications.Client.Model
{
    /// <summary>
    /// Device registration error codes
    /// </summary>
    public enum DeviceRegistrationErrorCodeContract
    {
        /// <summary>
        /// No error, represents a successful operation.
        /// </summary>
        None,

        /// <summary>
        /// The attempt in unregistering the device token has failed because the token does not exist.
        /// </summary>
        DoesNotExist,

        /// <summary>
        /// The attempt in registering device token has failed becuase the device had already been registered. 
        /// </summary>
        AlreadyRegistered,

        /// <summary>
        /// The attempt in registering device token has failed because the token was not a valid FCM token. 
        /// </summary>
        DeviceTokenNotValid,
        
        /// <summary>
        /// The attempt in registering device token has failed because unsupported locale was provided.
        /// </summary>
        UnsupportedLocale,
        
        /// <summary>
        /// One of the required fields is null or empty.
        /// </summary>
        InvalidInput,
    }
}
