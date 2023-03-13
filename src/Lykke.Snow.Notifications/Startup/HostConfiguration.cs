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
            var hostBuilder = builder.Host
                .UseServiceProviderFactory(new AutofacServiceProviderFactory())
                .ConfigureContainer<ContainerBuilder>((ctx, cBuilder) =>
                {
                    // register Autofac modules here
                    cBuilder.RegisterModule(new ServiceModule());
                })
                .UseSerilog((_, cfg) => cfg.ReadFrom.Configuration(configuration));
            
            return hostBuilder;
        }
    }
}


