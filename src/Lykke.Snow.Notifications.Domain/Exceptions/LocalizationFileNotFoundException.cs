using System.IO;

namespace Lykke.Snow.Notifications.Domain.Exceptions
{
    public class LocalizationFileNotFoundException : FileNotFoundException
    {
        public LocalizationFileNotFoundException(string attemptedPath) :
            base($"Localization file could not be found at {attemptedPath}", attemptedPath)
        {
        }
    }
}
