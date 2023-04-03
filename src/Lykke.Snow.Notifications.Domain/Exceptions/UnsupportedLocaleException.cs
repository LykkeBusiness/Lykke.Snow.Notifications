using System;

namespace Lykke.Snow.Notifications.Domain.Exceptions
{
    public sealed class UnsupportedLocaleException : Exception 
    {
        public UnsupportedLocaleException(string? locale) : base($"Locale [{locale}] is not supported")
        {
            if (locale != null)
            {
                Data.Add("Locale", locale);
            }
        }
    }
}
