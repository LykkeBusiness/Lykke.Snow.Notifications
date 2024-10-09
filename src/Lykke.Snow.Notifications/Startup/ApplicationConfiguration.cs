using System;
using Lykke.Middlewares;
using Lykke.Snow.Common.AssemblyLogging;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Lykke.Snow.Notifications.Startup
{
    public static class ApplicationConfiguration
    {
        public static WebApplication Configure(this WebApplication app)
        {
             if (app.Environment.IsDevelopment())
             {
                 app.UseDeveloperExceptionPage();
             }
             else
             {
                 app.UseHsts();
             }

             app.UseAuthentication();
             app.UseAuthorization();

             app.UseMiddleware<ExceptionHandlerMiddleware>();

             app.UseSwagger();
             app.UseSwaggerUI(a => a.SwaggerEndpoint("/swagger/v1/swagger.json", Program.ApiName));

             app.MapControllers();

             app.Lifetime.ApplicationStarted.Register(() =>
             {
                 var logger = app.Services.GetRequiredService<ILogger<Program>>();
                 try
                 {
                     app.Services.GetRequiredService<AssemblyLogger>()
                         .StartLogging();
                 }
                 catch (Exception e)
                 {
                     logger.LogError(e, "Failed to start");
                     app.Lifetime.StopApplication();
                     return;
                 }
                 logger.LogInformation($"{nameof(Startup)} started");
             });
             
             return app;
        }
    }
}
