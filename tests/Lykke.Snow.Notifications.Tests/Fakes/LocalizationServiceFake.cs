using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Snow.Notifications.Domain.Services;

namespace Lykke.Snow.Notifications.Tests.Fakes
{
    public class LocalizationServiceFake : ILocalizationService
    {
        public async Task<(string, string)> GetLocalizedTextAsync(string? notificationType, string? language, IReadOnlyList<string> parameters)
        {
            var title = $"title-{language}-{notificationType}";
            var body = $"body-{language}-{notificationType}";
            
            await Task.Run(() => Console.Write("localization is done"));
            
            return (title, body);
        }
    }
}
