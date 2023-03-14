using System;

namespace Lykke.Snow.Notifications.Domain.Exceptions
{
    public class FirebaseAppInitializationFailedException : Exception
    {
        public FirebaseAppInitializationFailedException(Exception innerException) : base("Couldn't initialize the Firebase App", innerException)
        { 
        }
    }
}
