using System;
using Autofac;
using Lykke.RabbitMqBroker;
using Lykke.Snow.Notifications.Settings;
using Lykke.Snow.Notifications.Subscribers;

namespace Lykke.Snow.Notifications.Modules
{
    public class RabbitMqModule : Module
    {
        private readonly NotificationServiceSettings _notificationServiceSettings;

        public RabbitMqModule(NotificationServiceSettings notificationServiceSettings)
        {
            _notificationServiceSettings = notificationServiceSettings ?? throw new ArgumentNullException(nameof(notificationServiceSettings));
        }

        protected override void Load(ContainerBuilder builder)
        {
            if(_notificationServiceSettings.Subscribers == null)
                throw new ArgumentNullException(nameof(_notificationServiceSettings.Subscribers));

            builder.RegisterType<MessagePreviewSubscriber>()
                .WithParameter(TypedParameter.From(_notificationServiceSettings.Subscribers.MessagePreviewSubscriber))
                .As<IStartStop>()
                .SingleInstance();
        }
    }
}
