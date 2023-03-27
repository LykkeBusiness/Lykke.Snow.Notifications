using System;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Lykke.SettingsReader;
using Lykke.Snow.Notifications.Modules;
using Lykke.Snow.Notifications.Settings;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace Lykke.Snow.Notifications.Startup
{
    public static class HostConfiguration
    {
        public static IHostBuilder ConfigureHost(this WebApplicationBuilder builder, IConfiguration configuration, IReloadingManager<AppSettings> settings)
        {
            if(settings.CurrentValue.NotificationService == null)
                throw new ArgumentException($"{nameof(AppSettings.NotificationService)} settings is not configured!");

            var hostBuilder = builder.Host
                .UseServiceProviderFactory(new AutofacServiceProviderFactory())
                .ConfigureContainer<ContainerBuilder>((ctx, cBuilder) =>
                {
                    // register Autofac modules here
                    cBuilder.RegisterModule(new ServiceModule());
                    cBuilder.RegisterModule(new NotificationsModule(settings.CurrentValue.NotificationService));
                    
                    // @atarutin: Due to a known bug in ASP.NET Core since version 3.0 ConfigureTestContainer is not
                    // being called when using WebApplicationFactory for integration testing, therefore "environment
                    // name" approach is used here to not register environment-specific modules in test environment.
                    // Once the bug is fixed ConfigureTestContainer should be used instead
                    // LINK: https://github.com/dotnet/aspnetcore/issues/14907
                    if (!ctx.HostingEnvironment.IsEnvironment("integration-tests"))
                    {
                        cBuilder.RegisterModule(new DalModule(settings.CurrentValue.NotificationService.Db.ConnectionString));
                        cBuilder.RegisterModule(new CqrsModule(settings.CurrentValue.NotificationService.Cqrs));
                    }
                })
                .UseSerilog((_, cfg) => cfg.ReadFrom.Configuration(configuration));
            
            return hostBuilder;
        }
    }
}


