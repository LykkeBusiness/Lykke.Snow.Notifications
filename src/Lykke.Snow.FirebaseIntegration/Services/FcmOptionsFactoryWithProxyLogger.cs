using FirebaseAdmin;
using Lykke.Snow.FirebaseIntegration.Interfaces;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Lykke.Snow.FirebaseIntegration.Services
{
    public sealed class FcmOptionsFactoryWithProxyLogger(
        IFcmOptionsFactory decoratee, 
        ProxyConfiguration proxyConfiguration,
        ILogger<FcmOptionsFactoryWithProxyLogger> logger,
        IHostEnvironment hostEnvironment) : IFcmOptionsFactory
    {
        private readonly IFcmOptionsFactory _decoratee = decoratee;
        private readonly ProxyConfiguration _proxyConfiguration = proxyConfiguration;
        private readonly ILogger<FcmOptionsFactoryWithProxyLogger> _logger = logger;
        private readonly IHostEnvironment _hostEnvironment = hostEnvironment;

        public AppOptions Create()
        {
            if (_hostEnvironment.IsProduction())
                return _decoratee.Create();

            _logger.LogWarning("Creating FCM options with proxy configuration: {@ProxyConfiguration}", _proxyConfiguration);
            return _decoratee.Create();
        }
    }
}
