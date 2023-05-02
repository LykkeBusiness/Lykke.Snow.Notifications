using System.Net.Http;
using Lykke.Cqrs;
using Lykke.Snow.FirebaseIntegration.Interfaces;
using Lykke.Snow.Notifications.Domain.Repositories;
using Lykke.Snow.Notifications.SqlRepositories;
using Lykke.Snow.Notifications.SqlRepositories.Repositories;
using Lykke.Snow.Notifications.Tests.Fakes;
using Lykke.Snow.Notifications.Tests.Repository;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace Lykke.Snow.Notifications.Tests
{
    public class WebAppFactoryNoDependencies : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("integration-tests");
            builder.UseSetting("SettingsUrl", "appsettings.test.json");
            
            builder.ConfigureServices(services =>
            {
                // register mocked cqrs engine
                services.AddSingleton(new Mock<ICqrsEngine>().Object);
                
                var contextFactory = new MsSqlContextFactoryInMemory();
                services.AddSingleton(contextFactory);
                services.AddSingleton<Lykke.Common.MsSql.IDbContextFactory<NotificationsDbContext>>(contextFactory);
                
                // register repositories manually since the DataLayer module registration was not called
                services.AddSingleton<IDeviceRegistrationRepository, DeviceRegistrationRepository>();
                services.AddSingleton<IDeviceConfigurationRepository, DeviceConfigurationRepository>();
                services.AddSingleton<IFcmIntegrationService, FcmIntegrationServiceFake>();
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
