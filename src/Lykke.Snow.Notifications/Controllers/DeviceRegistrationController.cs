using System.Threading.Tasks;
using Lykke.Snow.Contracts.Responses;
using Lykke.Snow.Notifications.Contracts.Model.Contracts;
using Lykke.Snow.Notifications.Contracts.Model.Requests;
using Microsoft.AspNetCore.Mvc;

namespace Lykke.Snow.Notifications.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DeviceRegistrationController : ControllerBase
    {
        [HttpPost("register")]
        public async Task<ErrorCodeResponse<DeviceRegistrationErrorCodes>> RegisterDevice(RegisterDeviceRequest request)
        {
            var response = new ErrorCodeResponse<DeviceRegistrationErrorCodes>();
            return response;
        }
    }
}
