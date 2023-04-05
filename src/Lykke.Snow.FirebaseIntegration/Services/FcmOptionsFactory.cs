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
            var options = new AppOptions { Credential = _credentialsProvider.Get() };

            if (_proxyConfiguration != null)
            {
                options.HttpClientFactory = new HttpClientFactoryWithProxy(_proxyConfiguration.Value);
            }

            return options;
        }
    }
}
