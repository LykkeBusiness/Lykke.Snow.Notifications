using System;
using Autofac;
using Lykke.Snow.FirebaseIntegration.Interfaces;
using Lykke.Snow.FirebaseIntegration.Services;
using Lykke.Snow.Notifications.Settings;

namespace Lykke.Snow.Notifications.Modules
{
    public class FirebaseModule : Module
    {
        private readonly NotificationServiceSettings _serviceSettings;

        public FirebaseModule(NotificationServiceSettings serviceSettings)
        {
            _serviceSettings = serviceSettings ?? throw new ArgumentNullException(nameof(serviceSettings));
        }

        protected override void Load(ContainerBuilder builder)
        {
            if(_serviceSettings.Fcm == null)
                throw new ArgumentNullException(nameof(_serviceSettings.Fcm));

            if(_serviceSettings.Fcm.CredentialFilePath == null)
                throw new ArgumentNullException(nameof(_serviceSettings.Fcm.CredentialFilePath));

            builder.RegisterType<FcmIntegrationService>()
                .WithParameter("credentialsFilePath", _serviceSettings.Fcm.CredentialFilePath)
                .As<IFcmIntegrationService>()
                .SingleInstance();
        }
    }
}
