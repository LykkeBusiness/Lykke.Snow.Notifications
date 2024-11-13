using FirebaseAdmin;
using Lykke.Snow.FirebaseIntegration.Interfaces;

namespace Lykke.Snow.FirebaseIntegration.Services
{
    public sealed class FcmOptionsFactoryWithProxy(
        IGoogleCredentialsProvider credentialsProvider,
        ProxyConfiguration proxyConfiguration) : IFcmOptionsFactory
    {
        private readonly IGoogleCredentialsProvider _credentialsProvider = credentialsProvider;
        private readonly ProxyConfiguration _proxyConfiguration = proxyConfiguration;

        public AppOptions Create()
        {
            var httpClientFactory = new HttpClientFactoryWithProxy(_proxyConfiguration);

            return new AppOptions
            {
                Credential = _credentialsProvider.Get().CreateWithHttpClientFactory(httpClientFactory),
                HttpClientFactory = httpClientFactory
            };
        }
    }
}
