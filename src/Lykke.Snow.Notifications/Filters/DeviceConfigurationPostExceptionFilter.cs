using System;
using AutoMapper;
using Lykke.Snow.Contracts.Responses;
using Lykke.Snow.Notifications.Client.Models;
using Lykke.Snow.Notifications.Controllers;
using Lykke.Snow.Notifications.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Lykke.Snow.Notifications.Filters
{
    /// <summary>
    /// An exception filter to handle exceptions in <see cref="DeviceConfigurationController.AddOrUpdate"/> method.
    /// </summary>
    public sealed class DeviceConfigurationPostExceptionFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            DeviceConfigurationErrorCodeContract? errorCode = context.Exception switch
            {
                ArgumentNullException _ => DeviceConfigurationErrorCodeContract.InvalidInput,
                UnsupportedLocaleException _ => DeviceConfigurationErrorCodeContract.UnsupportedLocale,
                DuplicateNotificationException _ => DeviceConfigurationErrorCodeContract.DuplicateNotificationType,
                UnsupportedNotificationTypeException _ => DeviceConfigurationErrorCodeContract.UnsupportedNotificationType,
                EntityNotFoundException _ => DeviceConfigurationErrorCodeContract.Conflict,
                EntityAlreadyExistsException _ => DeviceConfigurationErrorCodeContract.Conflict,
                AutoMapperMappingException { InnerException: ArgumentNullException _ } => DeviceConfigurationErrorCodeContract.InvalidInput,
                AutoMapperMappingException { InnerException: UnsupportedLocaleException _ } => DeviceConfigurationErrorCodeContract.UnsupportedLocale,
                AutoMapperMappingException { InnerException: DuplicateNotificationException _ } => DeviceConfigurationErrorCodeContract.DuplicateNotificationType,
                AutoMapperMappingException { InnerException: UnsupportedNotificationTypeException _ } => DeviceConfigurationErrorCodeContract.UnsupportedNotificationType,
                _ => null
            };

            if (errorCode == null) return;

            ErrorCodeResponse<DeviceConfigurationErrorCodeContract> response = errorCode.Value;
            context.ExceptionHandled = true;
            context.Result = new OkObjectResult(response);
        }
    }
}
