using Autofac;
using Lykke.Snow.Notifications.Domain.Services;
using Lykke.Snow.Notifications.DomainServices.Services;
using Lykke.Snow.Notifications.Settings;
using LykkeBiz.FirebaseIntegration.Interfaces;
using LykkeBiz.FirebaseIntegration.Services;

namespace Lykke.Snow.Notifications.Modules
{
    public class NotificationsModule : Module
    {
        private readonly NotificationServiceSettings serviceSettings;

        public NotificationsModule(NotificationServiceSettings serviceSettings)
        {
            this.serviceSettings = serviceSettings;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<NotificationService>()
                .As<INotificationService>()
                .SingleInstance();


            builder.RegisterType<FcmIntegrationService>()
                .WithParameter("credentialsFilePath", serviceSettings.Fcm.CredentialFilePath)
                .As<IFcmIntegrationService>()
                .SingleInstance();
        }
    }
}
