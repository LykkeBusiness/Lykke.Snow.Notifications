using System;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Lykke.Snow.Contracts.Responses;
using Lykke.Snow.Notifications.Client;
using Lykke.Snow.Notifications.Client.Models;
using Lykke.Snow.Notifications.Domain.Repositories;
using Lykke.Snow.Notifications.SqlRepositories.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Lykke.Snow.Notifications.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class DeviceConfigurationController : ControllerBase, IConfigurationApi
    {
        private readonly IDeviceConfigurationRepository _repository;
        private readonly IMapper _mapper;

        public DeviceConfigurationController(IDeviceConfigurationRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        [HttpGet("{deviceId}")]
        [ProducesResponseType(typeof(DeviceConfigurationResponse), (int)HttpStatusCode.OK)]
        public async Task<DeviceConfigurationResponse> Get([FromRoute] string deviceId)
        {
            try
            {
                var deviceConfiguration = await _repository.GetAsync(deviceId);
                var contract = _mapper.Map<DeviceConfigurationContract>(deviceConfiguration);

                return new DeviceConfigurationResponse(contract);
            }
            catch (EntityNotFoundException)
            {
                return DeviceConfigurationErrorCodeContract.DoesNotExist;
            }
        }

        [HttpDelete("{deviceId}")]
        [ProducesResponseType(typeof(ErrorCodeResponse<DeviceConfigurationErrorCodeContract>), (int)HttpStatusCode.OK)]
        public async Task<ErrorCodeResponse<DeviceConfigurationErrorCodeContract>> Delete([FromRoute] string deviceId)
        {
            try
            {
                await _repository.RemoveAsync(deviceId);
            }
            catch (EntityNotFoundException)
            {
                return DeviceConfigurationErrorCodeContract.DoesNotExist;
            }

            return DeviceConfigurationErrorCodeContract.None;
        }

        [HttpPost]
        [ProducesResponseType(typeof(ErrorCodeResponse<DeviceConfigurationErrorCodeContract>), (int)HttpStatusCode.OK)]
        public async Task<ErrorCodeResponse<DeviceConfigurationErrorCodeContract>> AddOrUpdate(
            DeviceConfigurationContract deviceConfiguration)
        {
            try
            {
                var dc = _mapper.Map<DeviceConfiguration>(deviceConfiguration);
                await _repository.AddOrUpdateAsync(dc);

                return DeviceConfigurationErrorCodeContract.None;
            }
            catch (AutoMapperMappingException e) when (e.InnerException is ArgumentException)
            {
                return DeviceConfigurationErrorCodeContract.InvalidInput;
            }
            catch (EntityNotFoundException)
            {
                return DeviceConfigurationErrorCodeContract.Conflict;
            }
            catch (EntityAlreadyExistsException)
            {
                return DeviceConfigurationErrorCodeContract.Conflict;
            }
        }
    }
}
