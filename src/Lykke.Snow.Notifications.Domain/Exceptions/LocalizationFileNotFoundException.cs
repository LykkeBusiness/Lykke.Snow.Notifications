using System;

namespace Lykke.Snow.Notifications.Domain.Exceptions
{
    public class LocalizationFileNotFoundException : Exception
    {
        public LocalizationFileNotFoundException(string attemptedPath) :
            base($"Localization file could not be found at {attemptedPath}")
        {
        }
    }
}
