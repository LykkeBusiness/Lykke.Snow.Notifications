using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lykke.Snow.Notifications.Domain.Repositories;
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
        
        // crud operations for device configuration
        private readonly IDeviceConfigurationRepository _repository;

        public DeviceConfigurationController(IDeviceConfigurationRepository repository)
        {
            _repository = repository;
        }

        [HttpPost]
        public async Task<IActionResult> AddOrUpdate([FromQuery] string deviceId,
            [FromQuery] string accountId, [FromBody] List<NotificationTypeConfig> notifications)
        {
            await _repository.AddOrUpdateAsync(new DeviceConfiguration(deviceId, accountId,
                notifications.Select(n => new DeviceConfiguration.Notification(n.TypeName, n.Enabled)).ToList()));

            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> Get(string deviceId)
        {
            var configuration = await _repository.GetAsync(deviceId);

            return Ok(configuration);
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(string deviceId)
        {
            await _repository.RemoveAsync(deviceId);

            return Ok();
        }
    }
}
