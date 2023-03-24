using System.Collections.Generic;

namespace Lykke.Snow.Notifications.Domain.Services
{
    public interface ILocalizationService
    {
        (string, string) GetLocalizedText(string notificationType, string language, IReadOnlyList<string> parameters);
    }
}
