using Lykke.Snow.Notifications.Client.Models;
using Lykke.Snow.Notifications.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Lykke.Snow.Notifications.Filters
{
    public sealed class DeviceConfigurationGetExceptionFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            DeviceConfigurationErrorCodeContract? errorCode = context.Exception switch
            {
                EntityNotFoundException _ => DeviceConfigurationErrorCodeContract.DoesNotExist,
                _ => null
            };
            
            if (errorCode == null) return;

            DeviceConfigurationResponse response = errorCode.Value;
            context.ExceptionHandled = true;
            context.Result = new OkObjectResult(response);
        }
    }
}