using System.Net.Http;
using Lykke.Common.MsSql;
using Lykke.Cqrs;
using Lykke.Snow.Notifications.Domain.Repositories;
using Lykke.Snow.Notifications.SqlRepositories;
using Lykke.Snow.Notifications.SqlRepositories.Repositories;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace Lykke.Snow.Notifications.Tests
{
    public class WebAppFactoryNoDependencies : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("test");
            builder.UseSetting("SettingsUrl", "appsettings.test.json");
            
            builder.ConfigureServices(services =>
            {
                // register mocked cqrs engine
                services.AddSingleton(new Mock<ICqrsEngine>().Object);
                
                // register in-memory database
                var options = new DbContextOptionsBuilder<NotificationsDbContext>()
                    .UseInMemoryDatabase("InMemoryNotificationsDb")
                    .Options;
                var contextFactory =
                    new MsSqlContextFactory<NotificationsDbContext>(_ => new NotificationsDbContext(options), options);
                services.AddSingleton(contextFactory);
                services.AddSingleton<Lykke.Common.MsSql.IDbContextFactory<NotificationsDbContext>>(contextFactory);
                
                // register repositories manually since the DataLayer module registration was not called
                services.AddSingleton<IDeviceRegistrationRepository, DeviceRegistrationRepository>();
                services.AddSingleton<IDeviceConfigurationRepository, DeviceConfigurationRepository>();
            });
        }
        
        /// <summary>
        /// Creates a new HttpClient with the default request headers set, in particular the api-key header.
        /// </summary>
        /// <returns></returns>
        public HttpClient CreateSecuredClient()
        {
            var client = CreateClient();
            client.DefaultRequestHeaders.Add("api-key", "test");
            return client;
        }
    }
}
