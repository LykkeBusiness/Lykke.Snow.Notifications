using System;

namespace Lykke.Snow.Notifications.Domain.Exceptions
{
    public class CredentialsFileNotFoundException : Exception
    {
        private const string ErrorMsg = "FCM credential file was not found at the given location.";

        public CredentialsFileNotFoundException(string attemptedPath) : base(ErrorMsg)
        {
            Data.Add(nameof(attemptedPath), attemptedPath);
        }
    }
}
