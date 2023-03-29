using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lykke.Snow.Notifications.Domain.Exceptions;
using Lykke.Snow.Notifications.Domain.Model;
using Lykke.Snow.Notifications.Domain.Services;
using Microsoft.Extensions.Logging;

namespace Lykke.Snow.Notifications.DomainServices.Services
{
    public class LocalizationService : ILocalizationService
    {
        private readonly ILocalizationDataProvider _localizationDataProvider;
        private ILogger<LocalizationService> _logger;
        
        private LocalizationData? _localizationData;

        public LocalizationService(ILogger<LocalizationService> logger, 
            ILocalizationDataProvider localizationDataProvider)
        {
            _logger = logger;
            _localizationDataProvider = localizationDataProvider;
        }

        public async Task<(string, string)> GetLocalizedTextAsync(string notificationType, string language, IReadOnlyList<string> parameters)
        {
            if(_localizationData == null)
                _localizationData = await _localizationDataProvider.Load();

            try 
            {
                language = language.ToLower();

                var title = _localizationData.Titles[notificationType][language];
                var body = _localizationData.Bodies[notificationType][language];
                
                body = string.Format(body, parameters.ToArray());
                
                return (title, body);
            }
            catch(KeyNotFoundException)
            {
                var ex = new TranslationNotFoundException(notificationType, language);
                _logger.LogError(ex, ex.Message);
                throw ex;
            }
            catch(NullReferenceException)
            {
                var ex = new TranslationNotFoundException(notificationType, language);
                _logger.LogError(ex, ex.Message);
                throw ex;
            }
            catch(FormatException)
            {
                var ex = new LocalizationFormatException(notificationType, language, _localizationData.Bodies[notificationType][language], parameters);
                _logger.LogError(ex, ex.Message);
                throw ex;
            }
        }
    }
}
