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
        private readonly ILogger<LocalizationService> _logger;
        private readonly HashSet<string> _translateAttributes;

        public LocalizationService(ILogger<LocalizationService> logger,
            ILocalizationDataProvider localizationDataProvider,
            string[] translateAttributes)
        {
            _logger = logger;
            _localizationDataProvider = localizationDataProvider;
            _translateAttributes = new HashSet<string>(translateAttributes);
        }

        public async Task<(string, string)> GetLocalizedTextAsync(string? notificationType, string? language, IList<string> parameters)
        {
            var localizationData = await _localizationDataProvider.Load();
            
            if(notificationType == null)
                throw new ArgumentNullException(nameof(notificationType));

            if(language == null)
                throw new ArgumentNullException(nameof(language));

            try 
            {
                language = language.ToLower();

                var title = localizationData.Titles[notificationType][language];
                var body = localizationData.Bodies[notificationType][language];
                
                parameters = TranslateParametersIfApplicable(localizationData, _translateAttributes, parameters, language);

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
                var ex = new LocalizationFormatException(notificationType, language, localizationData.Bodies[notificationType][language], parameters);

                _logger.LogError(ex, ex.Message);

                throw ex;
            }
        }
        
        public IList<string> TranslateParametersIfApplicable(
            LocalizationData localizationData,
            HashSet<string> translateAttributes,
            IList<string> parametersArg, 
            string lang)
        {
            if(parametersArg == null)
                return new List<string>();

            for(int i = 0; i < parametersArg.Count; i++)
            {
                var p = parametersArg[i];
                
                if(translateAttributes.Contains(p))
                    parametersArg[i] = localizationData.Attributes[p][lang];
            }
            
            return parametersArg;
        }
    }
}
