using System;
using System.Threading.Tasks;
using Lykke.Snow.Notifications.Startup;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace Lykke.Snow.Notifications
{
    internal sealed class Program
    {
        public static string ApiName = "Notifications";

        public static async Task Main(string[] args)
        {
            await StartupWrapper.StartAsync(async() => 
            {
                var builder = WebApplication.CreateBuilder(args);
                var (configuration, settingsManager) = builder.BuildConfiguration();

                builder.Services.RegisterInfrastructureServices(settingsManager);

                builder.ConfigureHost(configuration, settingsManager);

                var app = builder.Build();

                var startupManager = app.Services.GetRequiredService<StartupManager>();
                await startupManager.Start();

                await app.Configure().RunAsync();
            });
        }
    }
}
