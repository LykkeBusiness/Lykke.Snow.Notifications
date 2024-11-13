using System;
using Autofac;
using Lykke.Snow.FirebaseIntegration;
using Lykke.Snow.FirebaseIntegration.Interfaces;
using Lykke.Snow.FirebaseIntegration.Services;
using Lykke.Snow.Notifications.Settings;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

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

            if (_serviceSettings.Proxy != null)
            {
                RegisterFcmOptionsFactoryWithProxy(builder, _serviceSettings.Proxy);
            }
            else
            {
                builder.RegisterType<FcmOptionsFactory>()
                    .As<IFcmOptionsFactory>()
                    .SingleInstance();
            }

            builder.RegisterType<FcmIntegrationService>()
                .As<IFcmIntegrationService>()
                .SingleInstance();
        }

        private static void RegisterFcmOptionsFactoryWithProxy(ContainerBuilder builder, ProxySettings proxySettings)
        {
            var proxyConfiguration = new ProxyConfiguration(
                proxySettings.Address,
                proxySettings.Username,
                proxySettings.Password);

            builder.RegisterType<FcmOptionsFactoryWithProxy>()
                .As<IFcmOptionsFactory>()
                .WithParameter("proxyConfiguration", proxyConfiguration)
                .SingleInstance();

            builder.RegisterDecorator<IFcmOptionsFactory>((ctx, parameters, decoratee) =>
                new FcmOptionsFactoryWithProxyLogger(
                    decoratee,
                    proxyConfiguration,
                    ctx.Resolve<ILogger<FcmOptionsFactoryWithProxyLogger>>(),
                    ctx.Resolve<IHostEnvironment>()));
        }
    }
}
