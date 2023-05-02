using FirebaseAdmin;
using Lykke.Snow.FirebaseIntegration.Interfaces;

namespace Lykke.Snow.FirebaseIntegration.Services
{
    public sealed class FcmOptionsFactory : IFcmOptionsFactory
    {
        private readonly IGoogleCredentialsProvider _credentialsProvider;
        private readonly ProxyConfiguration? _proxyConfiguration;

        public FcmOptionsFactory(IGoogleCredentialsProvider credentialsProvider,
            ProxyConfiguration? proxyConfiguration = null)
        {
            _credentialsProvider = credentialsProvider;
            _proxyConfiguration = proxyConfiguration;
        }

        public AppOptions Create()
        {
            if (_proxyConfiguration != null)
            {
                var httpClientFactory = new HttpClientFactoryWithProxy(_proxyConfiguration.Value);

                var options = new AppOptions 
                { 
                    Credential = _credentialsProvider.Get().CreateWithHttpClientFactory(httpClientFactory),
                    HttpClientFactory = httpClientFactory
                };
                
                return options;
            }
            else
            {
                var options = new AppOptions { Credential = _credentialsProvider.Get()};

                return options;
            }
        }
    }
}
