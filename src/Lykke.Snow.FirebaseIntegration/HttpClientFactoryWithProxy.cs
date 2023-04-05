using System.Net;
using System.Net.Http;
using Google.Apis.Http;

namespace Lykke.Snow.FirebaseIntegration
{
    public sealed class HttpClientFactoryWithProxy : HttpClientFactory
    {
        private readonly ProxyConfiguration _proxyConfiguration;

        public HttpClientFactoryWithProxy(ProxyConfiguration proxyConfiguration)
        {
            _proxyConfiguration = proxyConfiguration;
        }

        protected override HttpMessageHandler CreateHandler(CreateHttpClientArgs args)
        {
            var proxy = new WebProxy(_proxyConfiguration.Address);
            if (_proxyConfiguration.CanUseCredentials)
            {
                proxy.Credentials = new NetworkCredential(_proxyConfiguration.Username, _proxyConfiguration.Password);
            }

            return new HttpClientHandler { Proxy = proxy };
        }
    }
}
