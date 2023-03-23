using System.Threading.Tasks;
using AutoMapper;
using Lykke.Snow.Notifications.Domain.Repositories;
using Lykke.Snow.Notifications.SqlRepositories.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace Lykke.Snow.Notifications.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    // todo: authorization?
    public class DeviceConfigurationController : ControllerBase
    {
        public struct NotificationTypeConfig
        {
            public string TypeName { get; set; }
            public bool Enabled { get; set; }
        }
        
        private readonly IDeviceConfigurationRepository _repository;
        private readonly IMapper _mapper;

        public DeviceConfigurationController(IDeviceConfigurationRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }
        
        [HttpGet("{deviceId}")]
        public async Task<IActionResult> Get([FromRoute] string deviceId)
        {
            try
            {
                var deviceConfiguration = await _repository.GetAsync(deviceId);
                var response = _mapper.Map<Responses.DeviceConfigurationResponse>(deviceConfiguration);
                return Ok(response);
            }
            catch (EntityNotFoundException)
            {
                return NotFound();
            }
        }
        
        [HttpDelete("{deviceId}")]
        public async Task<IActionResult> Delete([FromRoute] string deviceId)
        {
            try
            {
                await _repository.RemoveAsync(deviceId);
            }
            catch (EntityNotFoundException)
            {
                return NotFound();
            }

            return NoContent();
        }

        // [HttpPost("{deviceId}")]
        // public async Task<IActionResult> AddOrUpdate([FromRoute] string deviceId, 
        //     [FromQuery] string accountId, 
        //     [FromQuery] string locale, 
        //     [FromBody] List<NotificationTypeConfig> notifications)
        // {
        //     var deviceConfiguration = new DeviceConfiguration(deviceId, accountId, locale);
        //     
        //     await _repository.AddOrUpdateAsync(new DeviceConfiguration(deviceId, accountId, locale,
        //         notifications.Select(n => new DeviceConfiguration.Notification(n.TypeName, n.Enabled)).ToList()));
        //
        //     return Ok();
        // }
    }
}
