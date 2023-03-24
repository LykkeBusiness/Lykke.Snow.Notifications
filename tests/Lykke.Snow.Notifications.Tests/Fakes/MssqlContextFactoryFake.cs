using System;
using Lykke.Common.MsSql;
using Lykke.Snow.Notifications.SqlRepositories;
using Lykke.Snow.Notifications.SqlRepositories.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Lykke.Snow.Notifications.Tests.Fakes
{
    public class MssqlContextFactoryFake : Lykke.Common.MsSql.IDbContextFactory<NotificationsDbContext>, IDisposable
    {
        private readonly NotificationsDbContext ctx;

        public MssqlContextFactoryFake()
        {
            var contextOptions = new DbContextOptionsBuilder<NotificationsDbContext>()
               .UseInMemoryDatabase("NotificationsDbTest")
               .ConfigureWarnings(b => b.Ignore(InMemoryEventId.TransactionIgnoredWarning))
               .Options;
            
            ctx = new NotificationsDbContext(contextOptions);

             ctx.DeviceRegistrations.Add(
                new DeviceRegistrationEntity() { DeviceToken = "device-token-1", AccountId = "account-id-1", DeviceId = "device-id", RegisteredOn = DateTime.UtcNow });

             ctx.SaveChanges();
        }

        public NotificationsDbContext CreateDataContext()
        {
            return ctx;
        }

        public NotificationsDbContext CreateDataContext(TransactionContext transactionContext)
        {
            return ctx;
        }

        public void Dispose()
        {
            ctx.Dispose();
        }
    }
}
