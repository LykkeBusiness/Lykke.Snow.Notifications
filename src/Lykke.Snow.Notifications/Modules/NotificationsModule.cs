using Autofac;
using Lykke.Snow.Notifications.Domain.Services;
using Lykke.Snow.Notifications.DomainServices;
using Lykke.Snow.Notifications.DomainServices.Services;

namespace Lykke.Snow.Notifications.Modules
{
    public class NotificationsModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<NotificationService>()
                .As<INotificationService>()
                .SingleInstance();


            builder.RegisterType<FcmService>()
                .As<IFcmService>()
                .SingleInstance();
        }
    }
}
