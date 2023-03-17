using System.Threading.Tasks;
using Lykke.Snow.Common.Model;
using Lykke.Snow.Notifications.Domain.Enums;
using Lykke.Snow.Notifications.Domain.Model;
using Lykke.Snow.Notifications.Domain.Repositories;
using Lykke.Snow.Notifications.Domain.Services;
using Microsoft.Extensions.Internal;

namespace Lykke.Snow.Notifications.DomainServices.Services
{
    public class DeviceRegistrationService : IDeviceRegistrationService
    {
        private readonly IDeviceRegistrationRepository _repository;
        private readonly ISystemClock _systemClock;

        public DeviceRegistrationService(IDeviceRegistrationRepository repository, ISystemClock systemClock)
        {
            _repository = repository;
            _systemClock = systemClock;
        }

        // TODO what if mobile client sends some string for clientId field?
        // check if there's such a client exist. (through account api?)
        public async Task<Result<DeviceRegistrationErrorCode>> RegisterDeviceAsync(DeviceRegistration deviceRegistration)
        {
            deviceRegistration.SetRegisteredOn(_systemClock);

            var result = await _repository.InsertAsync(deviceRegistration);
            
            return result;
        }

        public Task<Result<DeviceRegistrationErrorCode>> UnregisterDeviceAsync(DeviceRegistration deviceRegistration)
        {
            throw new System.NotImplementedException();
        }
    }
}
