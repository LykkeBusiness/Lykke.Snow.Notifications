using System;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Lykke.Snow.FirebaseIntegration.Exceptions;
using Lykke.Snow.FirebaseIntegration.Interfaces;

namespace Lykke.Snow.FirebaseIntegration.Services
{
    public sealed class FcmOptionsFactory : IFcmOptionsFactory
    {
        private readonly string _credentialsFilePath;
        private readonly ProxyConfiguration? _proxyConfiguration;

        public FcmOptionsFactory(string credentialsFilePath, ProxyConfiguration? proxyConfiguration = null)
        {
            if (string.IsNullOrWhiteSpace(credentialsFilePath))
                throw new ArgumentNullException(nameof(credentialsFilePath));

            if (!System.IO.File.Exists(credentialsFilePath))
                throw new FirebaseCredentialsFileNotFoundException(attemptedPath: credentialsFilePath);

            _credentialsFilePath = credentialsFilePath;
            _proxyConfiguration = proxyConfiguration;
        }

        public AppOptions Create()
        {
            var options = new AppOptions { Credential = GoogleCredential.FromFile(_credentialsFilePath) };

            if (_proxyConfiguration != null)
            {
                options.HttpClientFactory = new HttpClientFactoryWithProxy(_proxyConfiguration.Value);
            }

            return options;
        }
    }
}
