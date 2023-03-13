using Lykke.Middlewares;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;

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

             return app;
        }
    }
}
