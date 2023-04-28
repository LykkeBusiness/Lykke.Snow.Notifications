using System;
using Lykke.Common.MsSql;
using Lykke.Snow.Notifications.SqlRepositories;
using Lykke.Snow.Notifications.SqlRepositories.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Lykke.Snow.Notifications.Tests.Repository
{
    public class MsSqlContextFactoryInMemory : Lykke.Common.MsSql.IDbContextFactory<NotificationsDbContext>
    {
        private readonly DbContextOptions<NotificationsDbContext> _options;
        
        public MsSqlContextFactoryInMemory()
        {
            _options = new DbContextOptionsBuilder<NotificationsDbContext>()
                .UseInMemoryDatabase("NotificationsDbTest")
                .ConfigureWarnings(b => b.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options;

            Seed();
        }

        public NotificationsDbContext CreateDataContext()
        {
            return new NotificationsDbContext(_options);
        }

        public NotificationsDbContext CreateDataContext(TransactionContext transactionContext)
        {
            return new NotificationsDbContext(_options);
        }

        private void Seed()
        {
            using var context = new NotificationsDbContext(_options);
            
            context.DeviceRegistrations.Add(
                new DeviceRegistrationEntity
                {
                    DeviceToken = "device-token-1",
                    AccountId = "account-id-1",
                    DeviceId = "device-id",
                    RegisteredOn = DateTime.UtcNow
                });

            context.SaveChanges();
        }
    }
}
