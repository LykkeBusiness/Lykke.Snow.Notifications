using System;
using System.Collections.Generic;
using System.IO;
using Lykke.Snow.Notifications.Domain.Exceptions;
using Lykke.Snow.Notifications.Domain.Services;
using Lykke.Snow.Notifications.DomainServices.Model;
using Newtonsoft.Json;

namespace Lykke.Snow.Notifications.DomainServices.Services
{
    public class LocalizationService : ILocalizationService
    {
        private readonly LocalizationData _localizationData;

        public LocalizationService(string localizationFilePath)
        {
            _localizationData = Initialize(localizationFilePath);
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
                throw new LocalizationFileParsingException();
            }
        }

        public (string, string) GetLocalizedText(string notificationType, string language)
        {
            try 
            {
                var title = _localizationData.Titles[notificationType][language];
                var body = _localizationData.Bodies[notificationType][language];
                
                return (title, body);
            }
            catch(KeyNotFoundException)
            {
                throw new TranslationNotFoundException(notificationType, language);
            }
            catch(NullReferenceException)
            {
                throw new TranslationNotFoundException(notificationType, language);
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
