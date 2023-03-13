using System;
using Lykke.Common.Api.Contract.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.PlatformAbstractions;

namespace Lykke.Snow.Notifications.Controllers
{
    [Route("api/[controller]")]
    public class IsAliveController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(
                new IsAliveResponse
                {
                    Version = PlatformServices.Default.Application.ApplicationVersion,
                    Env = Environment.GetEnvironmentVariable("ENV_INFO"),
#if DEBUG
                    IsDebug = true,
#else
                    IsDebug = false,
#endif
                    Name = Program.ApiName
                }
            );
        }

    }
}
