using System;
using System.Linq;
using Lykke.Snow.Contracts.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Lykke.Snow.Notifications.Filters
{
    public class InvalidInputValidationFilter<T> : IActionFilter where T : Enum
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (context.ModelState.IsValid) return;
            
            if (Enum.TryParse(typeof(T), "InvalidInput", true, out var invalidInput))
            {
                context.Result = new OkObjectResult(new ErrorCodeResponse<T> { ErrorCode = (T)invalidInput! });
            }
            else
            {
                var errors = context.ModelState.Values
                    .SelectMany(x => x.Errors)
                    .Select(x => x.ErrorMessage)
                    .ToList();
                
                context.Result = new BadRequestObjectResult(new { ErrorCode = "InvalidInput", Errors = errors });
            }
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
        }
    }
}
