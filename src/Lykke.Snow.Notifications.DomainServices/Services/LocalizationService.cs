using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Lykke.Snow.Notifications.Domain.Exceptions;
using Lykke.Snow.Notifications.Domain.Services;
using Lykke.Snow.Notifications.DomainServices.Model;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Lykke.Snow.Notifications.DomainServices.Services
{
    public class LocalizationService : ILocalizationService
    {
        private readonly LocalizationData _localizationData;
        private ILogger<LocalizationService> _logger;

        public LocalizationService(string localizationFilePath, 
            ILogger<LocalizationService> logger)
        {
            _localizationData = Initialize(localizationFilePath);
            _logger = logger;
        }

        // Constructor for unit tests
        public LocalizationService(LocalizationData localizationData, 
            ILogger<LocalizationService> logger)
        {
            _localizationData = localizationData;
            _logger = logger;
        }

        private LocalizationData Initialize(string localizationFilePath)
        {
            ThrowIfPathIsNotValid(localizationFilePath);

            var jsonText = File.ReadAllText(localizationFilePath);
            
            try
            {
                var result = JsonConvert.DeserializeObject<LocalizationData>(jsonText);
                
                if(result == null)
                    throw new LocalizationFileParsingException();

                ThrowIfDataIsNotValid(result);
                
                return result;
            }
            catch(JsonReaderException)
            {
                var ex = new LocalizationFileParsingException();
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }

        public (string, string) GetLocalizedText(string notificationType, string language, IReadOnlyList<string> parameters)
        {
            try 
            {
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
                var ex = new LocalizationFormatException(notificationType, language);
                _logger.LogError(ex, ex.Message);
                throw ex;
            }
        }
        
        private void ThrowIfPathIsNotValid(string localizationFilePath)
        {
            if (string.IsNullOrEmpty(localizationFilePath))
                throw new ArgumentNullException(nameof(localizationFilePath));
            
            if (!File.Exists(localizationFilePath))
                throw new LocalizationFileNotFoundException(localizationFilePath);
        }
        
        private void ThrowIfDataIsNotValid(LocalizationData localizationData)
        {
            if(localizationData.Titles == null)
                throw new LocalizationFileParsingException();

            if(localizationData.Bodies == null)
                throw new LocalizationFileParsingException();
        }
    }
}
