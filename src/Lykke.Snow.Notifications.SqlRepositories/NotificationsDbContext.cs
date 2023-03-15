using System.Data.Common;
using Lykke.Common.MsSql;
using Microsoft.EntityFrameworkCore;

namespace Lykke.Snow.Notifications.SqlRepositories
{
    public class NotificationsDbContext : MsSqlContext
    {
        private const string Schema = "notifications";
        
        public NotificationsDbContext() : base(Schema)
        {
        }
        
        public NotificationsDbContext(string connectionString, bool isTracingEnabled) : base(Schema, connectionString, isTracingEnabled)
        {
        }
        
        public NotificationsDbContext(DbContextOptions contextOptions) : base(Schema, contextOptions)
        {
        }
        
        public NotificationsDbContext(DbConnection dbConnection) : base(Schema, dbConnection)
        {
        }

        protected override void OnLykkeModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(NotificationsDbContext).Assembly);
        }
    }
}
