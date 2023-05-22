using System;

namespace Lykke.Snow.Notifications.Domain.Exceptions
{
    public class LocalizationFileCannotBeLoadedException : Exception
    {
        public LocalizationFileCannotBeLoadedException(string message) : base(message)
        {
        }
    }
}
