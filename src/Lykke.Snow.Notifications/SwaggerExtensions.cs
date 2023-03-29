using System.IO;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.PlatformAbstractions;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Lykke.Snow.Notifications
{
    internal static class SwaggerExtensions
    {
        public static void EnableXmlDocumentation(this SwaggerGenOptions swaggerOptions)
        {
            var basePath = PlatformServices.Default.Application.ApplicationBasePath;
            var entryAssembly = Assembly.GetEntryAssembly();
            if (entryAssembly == null)
            {
                return;
            }

            var xmlPath = Path.Combine(basePath, $"{entryAssembly.GetName().Name}.xml");

            if (File.Exists(xmlPath))
            {
                swaggerOptions.IncludeXmlComments(xmlPath);
            }
        }
    }
}
