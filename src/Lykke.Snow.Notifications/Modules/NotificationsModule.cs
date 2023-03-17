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
            _serviceSettings = serviceSettings;
        }

        protected override void Load(ContainerBuilder builder)
        {
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
