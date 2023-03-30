using System;
using System.IO;
using System.Threading.Tasks;
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

        public async Task<LocalizationData> Load()
        {
            ThrowIfPathIsNotValid(_localizationFilePath);

            var jsonText = await File.ReadAllTextAsync(_localizationFilePath);
            
            try
            {
                var result = JsonConvert.DeserializeObject<LocalizationData>(jsonText);
                
                if(result == null)
                    throw new LocalizationFileParsingException();
                
                return result;
            }
            catch(JsonReaderException)
            {
                var ex = new LocalizationFileParsingException();
                _logger.LogError(ex, ex.Message);
                throw ex;
            }
        }

        private static void ThrowIfPathIsNotValid(string localizationFilePath)
        {
            if (!File.Exists(localizationFilePath))
                throw new LocalizationFileNotFoundException(localizationFilePath);
        }
    }
}
