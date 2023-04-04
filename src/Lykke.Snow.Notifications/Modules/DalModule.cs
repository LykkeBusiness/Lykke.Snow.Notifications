using System;
using Autofac;
using Lykke.Common.MsSql;
using Lykke.Snow.Notifications.Domain.Repositories;
using Lykke.Snow.Notifications.DomainServices.Services;
using Lykke.Snow.Notifications.Settings;
using Lykke.Snow.Notifications.SqlRepositories;
using Lykke.Snow.Notifications.SqlRepositories.Repositories;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace Lykke.Snow.Notifications.Modules
{
    public class DalModule : Module
    {
        private readonly NotificationServiceSettings _notificationServiceSettings;

        public DalModule(NotificationServiceSettings notificationServiceSettings)
        {
            _notificationServiceSettings = notificationServiceSettings ??
                                           throw new ArgumentNullException(nameof(notificationServiceSettings));
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterMsSql(_notificationServiceSettings.Db.ConnectionString,
                connStr => new NotificationsDbContext(connStr, isTracingEnabled: false), 
                dbConnection => new NotificationsDbContext(dbConnection));
        
            builder.RegisterType<DeviceRegistrationRepository>()
                .As<IDeviceRegistrationRepository>()
                .SingleInstance();

            builder.RegisterType<DeviceConfigurationRepository>()
                .As<IDeviceConfigurationRepository>()
                .SingleInstance();

            builder.RegisterDecorator<IDeviceConfigurationRepository>((ctx, decoratee) => new DeviceConfigurationCache(
                    decoratee,
                    ctx.Resolve<IMemoryCache>(),
                    _notificationServiceSettings.ConfigurationCache?.ExpirationPeriod,
                    ctx.Resolve<ILogger<DeviceConfigurationCache>>()),
                fromKey: "DeviceConfigurationRepository");
        }
    }
}
