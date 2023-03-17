using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Lykke.Snow.Contracts.Responses;
using Lykke.Snow.Notifications.Contracts.Model.Contracts;
using Lykke.Snow.Notifications.Contracts.Model.Requests;
using Lykke.Snow.Notifications.Domain.Model;
using Lykke.Snow.Notifications.Domain.Services;
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
            var response = new ErrorCodeResponse<DeviceRegistrationErrorCodeContract>();

            var deviceRegistration = _mapper.Map<DeviceRegistration>(request);

            var result = await _deviceRegistrationService.RegisterDeviceAsync(deviceRegistration);
            
            // TODO: we can maybe create an extension method?
            //if(result.IsFailed)
            //    response.ErrorCode = _mapper.Map(...)
            
            return response;
        }

        [HttpPost("unregister")]
        [ProducesResponseType(typeof(ErrorCodeResponse<DeviceRegistrationErrorCodeContract>), (int) HttpStatusCode.OK)]
        public async Task<ErrorCodeResponse<DeviceRegistrationErrorCodeContract>> DeregisterDevice(UnregisterDeviceRequest request)
        {
            var response = new ErrorCodeResponse<DeviceRegistrationErrorCodeContract>();
            var deviceRegistration = _mapper.Map<DeviceRegistration>(request);
            
            //TODO check result
            await _deviceRegistrationService.UnregisterDeviceAsync(deviceRegistration);
            
            return response;
        }
    }
}
