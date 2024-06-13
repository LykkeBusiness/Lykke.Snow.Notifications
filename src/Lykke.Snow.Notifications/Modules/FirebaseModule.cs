using System;
using Autofac;
using Lykke.Snow.FirebaseIntegration;
using Lykke.Snow.FirebaseIntegration.Interfaces;
using Lykke.Snow.FirebaseIntegration.Services;
using Lykke.Snow.Notifications.Settings;

namespace Lykke.Snow.Notifications.Modules
{
    public class FirebaseModule(NotificationServiceSettings serviceSettings) : Module
    {
        private readonly NotificationServiceSettings _serviceSettings =
            serviceSettings ?? throw new ArgumentNullException(nameof(serviceSettings));

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<FileCredentials>()
                .WithParameter("credentialsFilePath", _serviceSettings.Fcm.CredentialFilePath)
                .As<IGoogleCredentialsProvider>()
                .SingleInstance();
            
            var fcmOptionsFactoryBuilder = builder.RegisterType<FcmOptionsFactory>()
                .As<IFcmOptionsFactory>()
                .SingleInstance();
            if (_serviceSettings.Proxy != null)
            {
                var proxyConfiguration = new ProxyConfiguration(_serviceSettings.Proxy.Address,
                    _serviceSettings.Proxy.Username,
                    _serviceSettings.Proxy.Password);
                fcmOptionsFactoryBuilder.WithParameter("proxyConfiguration", proxyConfiguration);
            }

            builder.RegisterType<FcmIntegrationService>()
                .As<IFcmIntegrationService>()
                .SingleInstance();
        }
    }
}
