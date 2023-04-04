using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Lykke.Snow.Contracts.Responses;
using Lykke.Snow.Notifications.Client;
using Lykke.Snow.Notifications.Client.Models;
using Lykke.Snow.Notifications.Domain.Model;
using Lykke.Snow.Notifications.Domain.Repositories;
using Lykke.Snow.Notifications.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Lykke.Snow.Notifications.Controllers
{
    /// <summary>
    /// Provides API for device configuration
    /// </summary>
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    [ServiceFilter(typeof(InvalidInputValidationFilter<DeviceConfigurationErrorCodeContract>))]
    public class DeviceConfigurationController : ControllerBase, IConfigurationApi
    {
        private readonly IDeviceConfigurationRepository _repository;
        private readonly IMapper _mapper;

        public DeviceConfigurationController(IDeviceConfigurationRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        /// <summary>
        /// Retrieves a device configuration by its ID.
        /// </summary>
        /// <param name="deviceId">Device id</param>
        /// <returns></returns>
        [HttpGet("{deviceId}")]
        [ProducesResponseType(typeof(DeviceConfigurationResponse), (int)HttpStatusCode.OK)]
        [ServiceFilter(typeof(DeviceConfigurationGetExceptionFilter))]
        public async Task<DeviceConfigurationResponse> Get([FromRoute] string deviceId)
        {
            var deviceConfiguration = await _repository.GetAsync(deviceId);
            var contract = _mapper.Map<DeviceConfigurationContract>(deviceConfiguration);

            return new DeviceConfigurationResponse(contract);
        }

        /// <summary>
        /// Deletes a device configuration by its ID.
        /// </summary>
        /// <param name="deviceId">Device id</param>
        /// <returns></returns>
        [HttpDelete("{deviceId}")]
        [ProducesResponseType(typeof(ErrorCodeResponse<DeviceConfigurationErrorCodeContract>), (int)HttpStatusCode.OK)]
        [ServiceFilter(typeof(DeviceConfigurationDeleteExceptionFilter))]
        public async Task<ErrorCodeResponse<DeviceConfigurationErrorCodeContract>> Delete([FromRoute] string deviceId)
        {
            await _repository.RemoveAsync(deviceId);

            return DeviceConfigurationErrorCodeContract.None;
        }

        /// <summary>
        /// Adds or updates a device configuration.
        /// </summary>
        /// <param name="deviceConfiguration"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(typeof(ErrorCodeResponse<DeviceConfigurationErrorCodeContract>), (int)HttpStatusCode.OK)]
        [ServiceFilter(typeof(DeviceConfigurationPostExceptionFilter))]
        public async Task<ErrorCodeResponse<DeviceConfigurationErrorCodeContract>> AddOrUpdate(
            DeviceConfigurationContract deviceConfiguration)
        {
            var dc = _mapper.Map<DeviceConfiguration>(deviceConfiguration);
            await _repository.AddOrUpdateAsync(dc);

            return DeviceConfigurationErrorCodeContract.None;
        }
    }
}
