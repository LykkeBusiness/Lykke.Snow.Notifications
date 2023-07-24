using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Lykke.Snow.Common.Model;
using Lykke.Snow.Contracts.Responses;
using Lykke.Snow.Notifications.Client;
using Lykke.Snow.Notifications.Client.Model;
using Lykke.Snow.Notifications.Client.Model.Requests;
using Lykke.Snow.Notifications.Domain.Enums;
using Lykke.Snow.Notifications.Domain.Model;
using Lykke.Snow.Notifications.Domain.Services;
using Lykke.Snow.Notifications.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Lykke.Snow.Notifications.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    [ServiceFilter(typeof(InvalidInputValidationFilter<DeviceRegistrationErrorCodeContract>))]
    public class DeviceRegistrationController : ControllerBase, INotificationsApi
    {
        private readonly IDeviceRegistrationService _deviceRegistrationService;
        private readonly IMapper _mapper;

        public DeviceRegistrationController(IDeviceRegistrationService deviceRegistrationService, IMapper mapper)
        {
            _deviceRegistrationService = deviceRegistrationService;
            _mapper = mapper;
        }

        [HttpPost]
        [ProducesResponseType(typeof(ErrorCodeResponse<DeviceRegistrationErrorCodeContract>), (int) HttpStatusCode.OK)]
        public async Task<ErrorCodeResponse<DeviceRegistrationErrorCodeContract>> RegisterDevice(RegisterDeviceRequest request)
        {
            var deviceRegistration = _mapper.Map<DeviceRegistration>(request);
            var result = await _deviceRegistrationService.RegisterDeviceAsync(deviceRegistration, 
                locale: request.Locale);
            
            return MapToResponse(result);
        }

        [HttpDelete]
        [ProducesResponseType(typeof(ErrorCodeResponse<DeviceRegistrationErrorCodeContract>), (int) HttpStatusCode.OK)]
        public async Task<ErrorCodeResponse<DeviceRegistrationErrorCodeContract>> UnregisterDevice(UnregisterDeviceRequest request)
        {
            var result = await _deviceRegistrationService.UnregisterDeviceAsync(deviceToken: request.DeviceToken);
            
            return MapToResponse(result);
        }

        private ErrorCodeResponse<DeviceRegistrationErrorCodeContract> MapToResponse(Result<DeviceRegistrationErrorCode> result)
        {
            return new ErrorCodeResponse<DeviceRegistrationErrorCodeContract> 
            { 
                ErrorCode = _mapper.Map<DeviceRegistrationErrorCodeContract>(result.Error) 
            };
        }
    }
}
