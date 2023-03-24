using System.Data.Common;
using Autofac;
using Lykke.Common.MsSql;
using Lykke.Snow.Notifications.Domain.Repositories;
using Lykke.Snow.Notifications.DomainServices.Services;
using Lykke.Snow.Notifications.SqlRepositories;
using Lykke.Snow.Notifications.SqlRepositories.Repositories;

namespace Lykke.Snow.Notifications.Modules
{
    public class DalModule : Module
    {
        private readonly string _connectionString;

        public DalModule(string connectionString)
        {
            _connectionString = connectionString;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterMsSql(_connectionString,
                (string connStr) => new NotificationsDbContext(connStr, isTracingEnabled: false), 
                (DbConnection dbConnection) => new NotificationsDbContext(dbConnection));
        
            builder.RegisterType<DeviceRegistrationRepository>()
                .As<IDeviceRegistrationRepository>()
                .SingleInstance();

            builder.RegisterType<DeviceConfigurationRepository>()
                .As<IDeviceConfigurationRepository>()
                .SingleInstance();
            
            builder.RegisterDecorator<DeviceConfigurationCache, IDeviceConfigurationRepository>();
        }
    }
}
