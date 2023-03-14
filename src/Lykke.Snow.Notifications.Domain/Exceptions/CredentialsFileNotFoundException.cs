using System;

namespace Lykke.Snow.Notifications.Domain.Exceptions
{
    public class CredentialsFileNotFoundException : Exception
    {
        public CredentialsFileNotFoundException(string attemptedPath) : base($"The FCM credentials file was not found {attemptedPath}")
        {
            Data.Add(nameof(attemptedPath), attemptedPath);
        }
    }
}
