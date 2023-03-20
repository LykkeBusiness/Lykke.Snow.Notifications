using System;
using System.Collections.Generic;
using System.Linq;
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

        public async Task<Result<DeviceRegistrationErrorCode>> UnregisterDeviceAsync(DeviceRegistration deviceRegistration)
        {
            var result = await _repository.GetDeviceRegistrationAsync(deviceToken: deviceRegistration.DeviceToken);
            
            if(result.IsFailed)
                return result;
            
            if(result.Value.AccountId != deviceRegistration.AccountId)
                return new Result<DeviceRegistrationErrorCode>(DeviceRegistrationErrorCode.AccountIdNotValid);
            
            return await _repository.DeleteAsync(deviceToken: deviceRegistration.DeviceToken);
        }

        public async Task<Result<IEnumerable<DeviceRegistration>, DeviceRegistrationErrorCode>> GetDeviceRegistrationsAsync(string accountId)
        {
            if(string.IsNullOrEmpty(accountId))
                throw new ArgumentNullException(nameof(accountId));

            var result = await _repository.GetDeviceRegistrationsByAccountIdAsync(accountId);
            
            if(result.IsFailed)
                return result;
            
            if(result == null)
                return new Result<IEnumerable<DeviceRegistration>, DeviceRegistrationErrorCode>(Enumerable.Empty<DeviceRegistration>());
            
            return result;
        }

    }
}
