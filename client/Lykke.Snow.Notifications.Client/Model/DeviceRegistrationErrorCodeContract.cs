namespace Lykke.Snow.Notifications.Client.Model
{
    /// <summary>
    /// Device registration error codes
    /// </summary>
    public enum DeviceRegistrationErrorCodeContract
    {
        /// <summary>
        /// No error
        /// </summary>
        None,
        /// <summary>
        /// The attempt in registering/unregistering the device token failed due to the token which does not exist.
        /// </summary>
        DoesNotExist,
        /// <summary>
        /// The attempt in registering device token failed becuase the device had already been registered. 
        /// </summary>
        AlreadyRegistered,
    }
}
