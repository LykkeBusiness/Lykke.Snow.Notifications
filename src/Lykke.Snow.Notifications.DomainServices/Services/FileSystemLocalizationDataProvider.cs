using System;
using System.IO;
using Lykke.Snow.Notifications.Domain.Exceptions;
using Lykke.Snow.Notifications.Domain.Model;
using Lykke.Snow.Notifications.Domain.Services;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Lykke.Snow.Notifications.DomainServices.Services
{
    public class FileSystemLocalizationDataProvider : ILocalizationDataProvider
    {
        private readonly string _localizationFilePath;
        private readonly ILogger<FileSystemLocalizationDataProvider> _logger;

        public FileSystemLocalizationDataProvider(string localizationFilePath, ILogger<FileSystemLocalizationDataProvider> logger)
        {
            _localizationFilePath = localizationFilePath ?? throw new ArgumentNullException(localizationFilePath);
            _logger = logger;
        }

        public LocalizationData Load()
        {
            ThrowIfPathIsNotValid(_localizationFilePath);

            var jsonText = File.ReadAllText(_localizationFilePath);
            
            try
            {
                var result = JsonConvert.DeserializeObject<LocalizationData>(jsonText);
                
                ThrowIfDataIsNotValid(result);
                
                return result;
            }
            catch(JsonReaderException)
            {
                var ex = new LocalizationFileParsingException();
                _logger.LogError(ex, ex.Message);
                throw ex;
            }
        }

        private void ThrowIfDataIsNotValid(LocalizationData localizationData)
        {
            if(localizationData == null)
                throw new LocalizationFileParsingException();
        }

        private static void ThrowIfPathIsNotValid(string localizationFilePath)
        {
            if (!File.Exists(localizationFilePath))
                throw new LocalizationFileNotFoundException(localizationFilePath);
        }
    }
}
