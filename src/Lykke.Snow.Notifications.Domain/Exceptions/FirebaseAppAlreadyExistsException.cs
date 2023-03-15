using System;

namespace Lykke.Snow.Notifications.Domain.Exceptions
{
    public class FirebaseAppAlreadyExistsException : Exception
    {
        private const string ErrorMsg = "FirebaseApp already exists! Initialize() should only be called once.";

        public FirebaseAppAlreadyExistsException(Exception innerException = null) : base(ErrorMsg, innerException)
        {
        }
    }
}
