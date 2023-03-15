using System;

namespace LykkeBiz.FirebaseIntegration.Exceptions
{
    public class FirebaseAppNotCreatedException : Exception
    {
        private const string ErrorMsg = "FirebaseApp has not been created yet.";
        
        public FirebaseAppNotCreatedException() : base(ErrorMsg)
        {
        }
    }
}
