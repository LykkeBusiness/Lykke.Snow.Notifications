using System;
using Lykke.SettingsReader;
using Lykke.Snow.Common.Startup;
using Lykke.Snow.Common.Startup.ApiKey;
using Lykke.Snow.Notifications.MappingProfiles;
using Lykke.Snow.Notifications.Settings;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace Lykke.Snow.Notifications.Startup
{
    public static class CompositionRoot
    {
        public static IServiceCollection RegisterInfrastructureServices(this IServiceCollection services, IReloadingManager<AppSettings> settings)
        {
            if(settings.CurrentValue.NotificationService == null)
                throw new ArgumentException($"{nameof(AppSettings.NotificationService)} settings is not configured!");

             services.AddSingleton(settings.CurrentValue.NotificationService);
             services
                .AddApplicationInsightsTelemetry()
                .AddMvcCore()
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.ContractResolver = new DefaultContractResolver();
                    options.SerializerSettings.Converters.Add(new StringEnumConverter());
                })
                .AddApiExplorer();

            services.AddControllers();
            services.AddAuthorization();
            services.AddApiKeyAuth(settings.CurrentValue.NotificationService.NotificationServiceClient);

            services.AddSwaggerGen(options =>
                {
                    options.SwaggerDoc(
                        "v1",
                        new OpenApiInfo { Version = "v1", Title = $"{Program.ApiName}" });

                    if (!string.IsNullOrWhiteSpace(settings.CurrentValue.NotificationService.NotificationServiceClient?.ApiKey))
                        options.AddApiKeyAwareness();
                })
                .AddSwaggerGenNewtonsoftSupport();
            
            services.AddAutoMapper(typeof(DeviceRegistrationMappingProfile).Assembly);
            
            return services;
        }

    }
}
