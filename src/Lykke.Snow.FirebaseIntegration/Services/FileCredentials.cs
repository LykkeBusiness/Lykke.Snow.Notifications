using System;
using Google.Apis.Auth.OAuth2;
using Lykke.Snow.FirebaseIntegration.Exceptions;
using Lykke.Snow.FirebaseIntegration.Interfaces;

namespace Lykke.Snow.FirebaseIntegration.Services
{
    public class FileCredentials : IGoogleCredentialsProvider
    {
        private readonly string _credentialsFilePath;

        public FileCredentials(string credentialsFilePath)
        {
            if (string.IsNullOrWhiteSpace(credentialsFilePath))
                throw new ArgumentNullException(nameof(credentialsFilePath));

            if (!System.IO.File.Exists(credentialsFilePath))
                throw new FirebaseCredentialsFileNotFoundException(attemptedPath: credentialsFilePath);
            
            _credentialsFilePath = credentialsFilePath;
        }

        public GoogleCredential Get() => GoogleCredential.FromFile(_credentialsFilePath);
    }
}
