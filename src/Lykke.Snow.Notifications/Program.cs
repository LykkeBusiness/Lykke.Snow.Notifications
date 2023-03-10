using System;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Lykke.Logs.Serilog;
using Lykke.Middlewares;
using Lykke.SettingsReader;
using Lykke.Snow.Common.Startup;
using Lykke.Snow.Notifications.Modules;
using Lykke.Snow.Notifications.Settings;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using Serilog;

namespace Lykke.Snow.Notifications
{
    internal sealed class Program
    {
        public static string ApiName = "Notifications";

        public static async Task Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .WriteTo.File("logs/service.start.log")
                .CreateBootstrapLogger();

            var restartAttemptsLeft = int.TryParse(Environment.GetEnvironmentVariable("RESTART_ATTEMPTS_NUMBER"),
                out var restartAttemptsFromEnv)
                ? restartAttemptsFromEnv
                : int.MaxValue;

            var restartAttemptsInterval = int.TryParse(Environment.GetEnvironmentVariable("RESTART_ATTEMPTS_INTERVAL_MS"),
                out var restartAttemptsIntervalFromEnv)
                ? restartAttemptsIntervalFromEnv
                : 10000;
        
            while(restartAttemptsLeft > 0)
            {
                try
                {
                    Log.Information("{Name} version {Version}", Assembly.GetEntryAssembly().GetName().Name, Assembly.GetEntryAssembly().GetName().Version.ToString());
                    Log.Information("ENV_INFO: {EnvInfo}", Environment.GetEnvironmentVariable("ENV_INFO"));

                    var builder = WebApplication.CreateBuilder(args);

                    builder.Environment.ContentRootPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

                    var configuration = builder.Configuration
                        .SetBasePath(builder.Environment.ContentRootPath)
                        .AddJsonFile("appsettings.json")
                        .AddSerilogJson(builder.Environment)
                        .AddUserSecrets<Program>()
                        .AddEnvironmentVariables()
                        .Build();

                    var settingsManager = configuration.LoadSettings<AppSettings>(_ => { });
                    var settings = settingsManager.CurrentValue.NotificationService ??
                        throw new ArgumentException($"{nameof(AppSettings.NotificationService)} settings is not configured!");
                    
                    builder.Services.AddSingleton(settings);

                     builder.Services
                        .AddApplicationInsightsTelemetry()
                        .AddMvcCore()
                        .AddNewtonsoftJson(options =>
                        {
                            options.SerializerSettings.ContractResolver = new DefaultContractResolver();
                            options.SerializerSettings.Converters.Add(new StringEnumConverter());
                        })
                        .AddApiExplorer();

                    builder.Services.AddControllers();
                    builder.Services.AddApiKeyAuth(settings.NotificationServiceClient);

                    builder.Services.AddSwaggerGen(options =>
                        {
                            options.SwaggerDoc(
                                "v1",
                                new OpenApiInfo { Version = "v1", Title = $"{ApiName}" });

                            // Add api key awareness if required
                        })
                        .AddSwaggerGenNewtonsoftSupport();

                    builder.Host
                        .UseServiceProviderFactory(new AutofacServiceProviderFactory())
                        .ConfigureContainer<ContainerBuilder>((ctx, cBuilder) =>
                        {
                            // register Autofac modules here
                            cBuilder.RegisterModule(new ServiceModule());
                        })
                        .UseSerilog((_, cfg) => cfg.ReadFrom.Configuration(configuration));

                    var app = builder.Build();

                    if (app.Environment.IsDevelopment())
                    {
                        app.UseDeveloperExceptionPage();
                    }
                    else
                    {
                        app.UseHsts();
                    }

                    app.UseMiddleware<ExceptionHandlerMiddleware>();

                    app.UseSwagger();
                    app.UseSwaggerUI(a => a.SwaggerEndpoint("/swagger/v1/swagger.json", ApiName));

                    app.MapControllers();

                    await app.RunAsync();
                    
                }
                catch (Exception e)
                {
                    Log.Fatal(e,
                      "Host terminated unexpectedly. Restart in {RestartAttemptsInterval} ms. Attempts left: {RestartAttemptsLeft}",
                      restartAttemptsInterval, restartAttemptsLeft);
                    restartAttemptsLeft--;
                    Thread.Sleep(restartAttemptsInterval);
                }
            }

            // Lets devops to see startup error in console between restarts in the Kubernetes
            var delay = TimeSpan.FromMinutes(1);

            Log.Information("Process will be terminated in {Delay}. Press any key to terminate immediately", delay);

            await Task.WhenAny(
                Task.Delay(delay),
                Task.Run(() => Console.ReadKey(true)));

            Log.Information("Terminated");
            Log.CloseAndFlush();
        }
    }
}
