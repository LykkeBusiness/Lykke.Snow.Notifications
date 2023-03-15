using System;

namespace LykkeBiz.FirebaseIntegration.Exceptions
{
    public class FirebaseCredentialsFileNotFoundException : Exception
    {
        private const string ErrorMsg = "FCM credential file was not found at the given location.";

        public FirebaseCredentialsFileNotFoundException(string attemptedPath) : base(ErrorMsg)
        {
            Data.Add(nameof(attemptedPath), attemptedPath);
        }
    }
}
