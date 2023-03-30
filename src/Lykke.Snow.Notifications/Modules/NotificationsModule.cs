using System;
using Autofac;
using Lykke.Snow.FirebaseIntegration.Interfaces;
using Lykke.Snow.FirebaseIntegration.Services;
using Lykke.Snow.Notifications.Domain.Services;
using Lykke.Snow.Notifications.DomainServices.Services;
using Lykke.Snow.Notifications.Settings;

namespace Lykke.Snow.Notifications.Modules
{
    public class NotificationsModule : Module
    {
        private readonly NotificationServiceSettings _serviceSettings;

        public NotificationsModule(NotificationServiceSettings serviceSettings)
        {
            _serviceSettings = serviceSettings ?? throw new ArgumentNullException(nameof(serviceSettings));
        }

        protected override void Load(ContainerBuilder builder)
        {
            if(_serviceSettings.Fcm == null)
                throw new ArgumentNullException(nameof(_serviceSettings.Fcm));

            if(_serviceSettings.Fcm.CredentialFilePath == null)
                throw new ArgumentNullException(nameof(_serviceSettings.Fcm.CredentialFilePath));

            builder.RegisterType<NotificationService>()
                .As<INotificationService>()
                .SingleInstance();

            builder.RegisterType<FcmIntegrationService>()
                .WithParameter("credentialsFilePath", _serviceSettings.Fcm.CredentialFilePath)
                .As<IFcmIntegrationService>()
                .SingleInstance();
        }
    }
}
