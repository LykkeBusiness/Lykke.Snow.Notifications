using System;
using Lykke.Snow.Notifications.Domain.Services;

namespace Lykke.Snow.Notifications.Domain.Exceptions
{
    public class FirebaseAppAlreadyExistsException : Exception
    {
        public FirebaseAppAlreadyExistsException(Exception innerException = null) : 
            base($"Firebase App already exists! Check if {nameof(INotificationService)} has been initialized twice.", innerException)
        {
        }
    }
}
