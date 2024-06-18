using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Lykke.Contracts.Responses;
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
        /// <param name="accountId">Account id</param>
        /// <returns></returns>
        [HttpGet("{deviceId}/{accountId}")]
        [ProducesResponseType(typeof(DeviceConfigurationResponse), (int)HttpStatusCode.OK)]
        [ServiceFilter(typeof(DeviceConfigurationGetExceptionFilter))]
        public async Task<DeviceConfigurationResponse> Get([FromRoute] string deviceId, [FromRoute] string accountId)
        {
            var deviceConfiguration = await _repository.GetAsync(deviceId, accountId);

            var contract = _mapper.Map<DeviceConfigurationContract>(deviceConfiguration);

            if(contract == null)
                return new DeviceConfigurationResponse(DeviceConfigurationErrorCodeContract.DoesNotExist);

            return new DeviceConfigurationResponse(contract);
        }

        /// <summary>
        /// Deletes a device configuration by its ID.
        /// </summary>
        /// <param name="deviceId">Device id</param>
        /// <param name="accountId">Account id</param>
        /// <returns></returns>
        [HttpDelete("{deviceId}/{accountId}")]
        [ProducesResponseType(typeof(ErrorCodeResponse<DeviceConfigurationErrorCodeContract>), (int)HttpStatusCode.OK)]
        [ServiceFilter(typeof(DeviceConfigurationDeleteExceptionFilter))]
        public async Task<ErrorCodeResponse<DeviceConfigurationErrorCodeContract>> Delete([FromRoute] string deviceId, [FromRoute] string accountId)
        {
            await _repository.RemoveAsync(deviceId, accountId);

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
