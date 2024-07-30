using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Autofac;
using FirebaseAdmin.Messaging;
using Lykke.Snow.Common.Model;
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

            builder.RegisterType<FcmIntegrationServiceFake>()
                .As<IFcmIntegrationService>()
                .SingleInstance();
        }
    }
    
    public class FcmIntegrationServiceFake : IFcmIntegrationService
    {
        private static bool _isDeviceTokenValid = true;

        public ConcurrentBag<Message> ReceivedMessages { get; } = new ConcurrentBag<Message>();
        public static void SetIsDeviceTokenValid(bool value) => _isDeviceTokenValid = value;

        public Task<bool> IsDeviceTokenValid(string deviceToken)
        {
            return Task.FromResult(_isDeviceTokenValid);
        }

        public Task<Result<string, MessagingErrorCode>> SendNotification(Message message)
        {
            ReceivedMessages.Add(message);
            return new Result<string, MessagingErrorCode>(string.Empty);
        }
    }
}
