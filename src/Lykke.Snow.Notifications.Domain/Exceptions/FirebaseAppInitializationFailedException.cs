using System;

namespace Lykke.Snow.Notifications.Domain.Exceptions
{
    public class FirebaseAppInitializationFailedException : Exception
    {
        private const string ErrorMsg = "Couldn't initialize the Firebase app.";

        public FirebaseAppInitializationFailedException(Exception innerException) : base(ErrorMsg, innerException)
        { 
        }
    }
}