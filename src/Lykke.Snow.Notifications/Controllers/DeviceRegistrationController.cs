using System.Threading.Tasks;
using AutoMapper;
using Lykke.Snow.Contracts.Responses;
using Lykke.Snow.Notifications.Contracts.Model.Contracts;
using Lykke.Snow.Notifications.Contracts.Model.Requests;
using Lykke.Snow.Notifications.Domain.Model;
using Lykke.Snow.Notifications.Domain.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Lykke.Snow.Notifications.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DeviceRegistrationController : ControllerBase
    {
        private readonly IDeviceRegistrationRepository _deviceRegistrationRepository;
        private readonly IMapper _mapper;

        public DeviceRegistrationController(IDeviceRegistrationRepository deviceRegistrationRepository, IMapper mapper)
        {
            _deviceRegistrationRepository = deviceRegistrationRepository;
            _mapper = mapper;
        }

        [HttpPost("register")]
        public async Task<ErrorCodeResponse<DeviceRegistrationErrorCodeContract>> RegisterDevice(RegisterDeviceRequest request)
        {
            var response = new ErrorCodeResponse<DeviceRegistrationErrorCodeContract>();
            var deviceRegistration = _mapper.Map<DeviceRegistration>(request);
            
            await _deviceRegistrationRepository.InsertAsync(deviceRegistration);
            
            return response;
        }

        [HttpPost("unregister")]
        public async Task<ErrorCodeResponse<DeviceRegistrationErrorCodeContract>> RegisterDevice(string deviceToken)
        {
            var response = new ErrorCodeResponse<DeviceRegistrationErrorCodeContract>();
            
            await _deviceRegistrationRepository.DeleteAsync(deviceToken);
            
            return response;
        }
    }
}
