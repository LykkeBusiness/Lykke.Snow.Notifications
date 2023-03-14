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
                    cBuilder.RegisterModule(new CqrsModule(settings.CurrentValue.NotificationService.Cqrs));
                    cBuilder.RegisterModule(new NotificationsModule());
                })
                .UseSerilog((_, cfg) => cfg.ReadFrom.Configuration(configuration));
            
            return hostBuilder;
        }
    }
}


