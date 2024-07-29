using System;
using Autofac;
using Lykke.RabbitMqBroker;
using Lykke.RabbitMqBroker.Subscriber;
using Lykke.Snow.Notifications.Domain.Model;
using Lykke.Snow.Notifications.DomainServices.Services;
using Lykke.Snow.Notifications.Settings;

namespace Lykke.Snow.Notifications.Modules
{
    public class RabbitMqModule(NotificationServiceSettings notificationServiceSettings) : Module
    {
        private readonly NotificationServiceSettings _notificationServiceSettings = notificationServiceSettings ??
            throw new ArgumentNullException(nameof(notificationServiceSettings));

        protected override void Load(ContainerBuilder builder)
        {
            if(_notificationServiceSettings.Subscribers?.MessagePreviewSubscriber == null)
                throw new ArgumentNullException(nameof(_notificationServiceSettings.Subscribers.MessagePreviewSubscriber));

            builder.AddRabbitMqConnectionProvider();

            builder.AddRabbitMqListener<MessagePreviewEvent, MessagePreviewHandler>(_notificationServiceSettings.Subscribers.MessagePreviewSubscriber)
                .AddOptions(RabbitMqListenerOptions<MessagePreviewEvent>.MessagePack.LossAcceptable)
                .AutoStart();
        }
    }
}
