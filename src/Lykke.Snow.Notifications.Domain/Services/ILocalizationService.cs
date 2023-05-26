using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lykke.Snow.Notifications.Domain.Services
{
    /// <summary>
    /// Responsible for localization matters across the service.
    /// </summary>
    public interface ILocalizationService
    {
        /// <summary>
        /// Returns the localized title and body based on type and language.
        /// </summary>
        /// <param name="notificationType"></param>
        /// <param name="language"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        Task<(string, string)> GetLocalizedTextAsync(string? notificationType, string? language, IList<string> parameters);
    }
}
