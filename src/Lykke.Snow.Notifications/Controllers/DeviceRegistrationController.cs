using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Lykke.Snow.Common.Model;
using Lykke.Snow.Contracts.Responses;
using Lykke.Snow.Notifications.Contracts.Model.Contracts;
using Lykke.Snow.Notifications.Contracts.Model.Requests;
using Lykke.Snow.Notifications.Domain.Enums;
using Lykke.Snow.Notifications.Domain.Model;
using Lykke.Snow.Notifications.Domain.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Lykke.Snow.Notifications.Controllers
{
    [ApiController]
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
        public async Task<ErrorCodeResponse<DeviceRegistrationErrorCodeContract>> DeregisterDevice(UnregisterDeviceRequest request)
        {
            var response = new ErrorCodeResponse<DeviceRegistrationErrorCodeContract>();
            var deviceRegistration = _mapper.Map<DeviceRegistration>(request);
            
            var result = await _deviceRegistrationService.UnregisterDeviceAsync(deviceRegistration);
            
            return MapToResponse(result, Response);
        }

        private ErrorCodeResponse<DeviceRegistrationErrorCodeContract> MapToResponse(Result<DeviceRegistrationErrorCode> result, HttpResponse response)
        {
            if(result.IsFailed)
                response.StatusCode = (int) HttpStatusCode.BadRequest;

            return new ErrorCodeResponse<DeviceRegistrationErrorCodeContract> 
            { 
                ErrorCode = _mapper.Map<DeviceRegistrationErrorCodeContract>(result.Error) 
            };
        }
    }
}
