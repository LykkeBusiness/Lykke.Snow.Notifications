using System.Data.Common;
using Autofac;
using Lykke.Common.MsSql;
using Lykke.Snow.Notifications.SqlRepositories;

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
        }
    }
}
