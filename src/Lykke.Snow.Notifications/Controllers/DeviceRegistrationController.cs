using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Lykke.Snow.Common.Model;
using Lykke.Snow.Contracts.Responses;
using Lykke.Snow.Notifications.Client.Model;
using Lykke.Snow.Notifications.Client.Model.Requests;
using Lykke.Snow.Notifications.Domain.Enums;
using Lykke.Snow.Notifications.Domain.Model;
using Lykke.Snow.Notifications.Domain.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Lykke.Snow.Notifications.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class DeviceRegistrationController : ControllerBase
    {
        private readonly IDeviceRegistrationService _deviceRegistrationService;
        private readonly IMapper _mapper;

        public DeviceRegistrationController(IDeviceRegistrationService deviceRegistrationService, IMapper mapper)
        {
            _deviceRegistrationService = deviceRegistrationService;
            _mapper = mapper;
        }

        [HttpPost("register")]
        [ProducesResponseType(typeof(ErrorCodeResponse<DeviceRegistrationErrorCodeContract>), (int) HttpStatusCode.OK)]
        public async Task<ErrorCodeResponse<DeviceRegistrationErrorCodeContract>> RegisterDevice(RegisterDeviceRequest request)
        {
            var deviceRegistration = _mapper.Map<DeviceRegistration>(request);
            var result = await _deviceRegistrationService.RegisterDeviceAsync(deviceRegistration);
            
            return MapToResponse(result, Response);
        }

        [HttpPost("unregister")]
        [ProducesResponseType(typeof(ErrorCodeResponse<DeviceRegistrationErrorCodeContract>), (int) HttpStatusCode.OK)]
        public async Task<ErrorCodeResponse<DeviceRegistrationErrorCodeContract>> UnregisterDevice(UnregisterDeviceRequest request)
        {
            var response = new ErrorCodeResponse<DeviceRegistrationErrorCodeContract>();
            
            var result = await _deviceRegistrationService.UnregisterDeviceAsync(deviceToken: request.DeviceToken);
            
            return MapToResponse(result, Response);
        }

        private ErrorCodeResponse<DeviceRegistrationErrorCodeContract> MapToResponse(Result<DeviceRegistrationErrorCode> result, HttpResponse response)
        {
            return new ErrorCodeResponse<DeviceRegistrationErrorCodeContract> 
            { 
                ErrorCode = _mapper.Map<DeviceRegistrationErrorCodeContract>(result.Error) 
            };
        }
    }
}
