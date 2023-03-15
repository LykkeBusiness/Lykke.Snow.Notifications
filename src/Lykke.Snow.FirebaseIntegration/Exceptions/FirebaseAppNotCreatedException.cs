using System;

namespace Lykke.Snow.FirebaseIntegration.Exceptions
{
    public class FirebaseAppNotCreatedException : Exception
    {
        private const string ErrorMsg = "FirebaseApp has not been created yet.";
        
        public FirebaseAppNotCreatedException() : base(ErrorMsg)
        {
        }
    }
}
