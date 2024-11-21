using System;
using System.IO;
using System.Reflection;
using Lykke.SettingsReader;
using Lykke.SettingsReader.ConfigurationProvider;
using Lykke.Snow.Notifications.Settings;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;

namespace Lykke.Snow.Notifications.Startup
{
    public static class ConfigurationBuilder
    {
        public static (IConfigurationRoot, IReloadingManager<AppSettings>) BuildConfiguration(this WebApplicationBuilder builder)
        {
            var assemblyLocation = Assembly.GetExecutingAssembly().Location;
            var assemblyPath = Path.GetDirectoryName(assemblyLocation);
            if (string.IsNullOrEmpty(assemblyPath))
            {
                throw new DirectoryNotFoundException("Failed to get directory of the executing assembly");
            }

            builder.Environment.ContentRootPath = assemblyPath;

            var configurationBuilder = builder.Configuration
                .SetBasePath(builder.Environment.ContentRootPath)
                .AddUserSecrets<Program>()
                .AddEnvironmentVariables();

            if (Environment.GetEnvironmentVariable("SettingsUrl")?.StartsWith("http") ?? false)
            {
                configurationBuilder.AddHttpSourceConfiguration();
            }

            var configuration = configurationBuilder.Build();

            var settingsManager = configuration.LoadSettings<AppSettings>(_ => { });

            return (configuration, settingsManager);
        }
    }
}
