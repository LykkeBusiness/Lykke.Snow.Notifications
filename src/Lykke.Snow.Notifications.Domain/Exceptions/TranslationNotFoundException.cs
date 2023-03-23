using System;

namespace Lykke.Snow.Notifications.Domain.Exceptions
{
    public class TranslationNotFoundException : Exception
    {
        public TranslationNotFoundException(string notificationType, string language) :
            base($"Translation data is not valid for {notificationType} - {language}")
        {
        }
    }
}
