using System;
using System.Reflection;
using System.Runtime.InteropServices;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Lykke.SettingsReader;
using Lykke.Snow.Notifications.Extensions;
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
                    cBuilder.RegisterModule(new ServiceModule(settings.CurrentValue.NotificationService));
                    cBuilder.RegisterModule(new NotificationsModule(settings.CurrentValue.NotificationService));
                    cBuilder.RegisterModule(new ExternalServicesModule(settings.CurrentValue.NotificationService));

                    // @atarutin: Due to a known bug in ASP.NET Core since version 3.0 ConfigureTestContainer is not
                    // being called when using WebApplicationFactory for integration testing, therefore "environment
                    // name" approach is used here to not register environment-specific modules in test environment.
                    // Once the bug is fixed ConfigureTestContainer should be used instead
                    // LINK: https://github.com/dotnet/aspnetcore/issues/14907
                    if (!ctx.HostingEnvironment.IsEnvironment("integration-tests"))
                    {
                        cBuilder.RegisterModule(new DalModule(settings.CurrentValue.NotificationService));
                        cBuilder.RegisterModule(new CqrsModule(settings.CurrentValue.NotificationService.Cqrs));
                        cBuilder.RegisterModule(new RabbitMqModule(settings.CurrentValue.NotificationService));
                        cBuilder.RegisterModule(new FirebaseModule(settings.CurrentValue.NotificationService));
                    }
                })
                .UseSerilog((_, cfg) =>
                {
                    var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
                    var assembly = typeof(Program).Assembly;
                    var title = assembly.Attribute<AssemblyTitleAttribute>(attribute => attribute.Title);
                    var version = assembly.Attribute<AssemblyInformationalVersionAttribute>(attribute => attribute.InformationalVersion);
                    var copyright = assembly.Attribute<AssemblyCopyrightAttribute>(attribute => attribute.Copyright);

                    cfg.ReadFrom.Configuration(configuration)
                        .Enrich.WithProperty("Application", title)
                        .Enrich.WithProperty("Version", version)
                        .Enrich.WithProperty("Environment", environmentName ?? "Development");
                    
                     Log.Information($"{title} [{version}] {copyright}");
                     Log.Information($"Running on: {RuntimeInformation.OSDescription}");
                });

            return hostBuilder;
        }
    }
}


