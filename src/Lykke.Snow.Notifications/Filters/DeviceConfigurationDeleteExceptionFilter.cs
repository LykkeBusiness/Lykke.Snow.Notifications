using Lykke.Contracts.Responses;
using Lykke.Snow.Notifications.Client.Models;
using Lykke.Snow.Notifications.Controllers;
using Lykke.Snow.Notifications.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Lykke.Snow.Notifications.Filters
{
    /// <summary>
    /// An exception filter to handle exceptions in <see cref="DeviceConfigurationController.Delete"/> method.
    /// </summary>
    public sealed class DeviceConfigurationDeleteExceptionFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            DeviceConfigurationErrorCodeContract? errorCode = context.Exception switch
            {
                EntityNotFoundException _ => DeviceConfigurationErrorCodeContract.DoesNotExist,
                _ => throw context.Exception
            };
            
            ErrorCodeResponse<DeviceConfigurationErrorCodeContract> response = errorCode.Value;
            context.ExceptionHandled = true;
            context.Result = new OkObjectResult(response);
        }
    }
}
