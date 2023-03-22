using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lykke.Snow.Common.Model;
using Lykke.Snow.Notifications.Domain.Enums;
using Lykke.Snow.Notifications.Domain.Exceptions;
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

        public async Task<Result<DeviceRegistrationErrorCode>> RegisterDeviceAsync(DeviceRegistration deviceRegistration)
        {
            try
            {
                await _repository.InsertAsync(deviceRegistration);
                return new Result<DeviceRegistrationErrorCode>();
            }
            catch(EntityAlreadyExistsException)
            {
                return new Result<DeviceRegistrationErrorCode>(DeviceRegistrationErrorCode.AlreadyRegistered);
            }
        }

        public async Task<Result<DeviceRegistrationErrorCode>> UnregisterDeviceAsync(string deviceToken, string accountId)
        {
            DeviceRegistration? result = null;
            try
            {
                result = await _repository.GetDeviceRegistrationAsync(deviceToken: deviceToken);
            }
            catch(EntityNotFoundException)
            {
                return new Result<DeviceRegistrationErrorCode>(DeviceRegistrationErrorCode.DoesNotExist);
            }
            
            if(result == null)
            {
                return new Result<DeviceRegistrationErrorCode>(DeviceRegistrationErrorCode.DoesNotExist);
            }

            if(result.AccountId != accountId)
                return new Result<DeviceRegistrationErrorCode>(DeviceRegistrationErrorCode.AccountIdNotValid);
            
            try
            {
               await _repository.DeleteAsync(oid: result.Oid);
               return new Result<DeviceRegistrationErrorCode>();
            }
            catch(EntityNotFoundException)
            {
                return new Result<DeviceRegistrationErrorCode>(DeviceRegistrationErrorCode.DoesNotExist);
            }
        }

        public async Task<Result<IEnumerable<DeviceRegistration>, DeviceRegistrationErrorCode>> GetDeviceRegistrationsAsync(string accountId)
        {
            if(string.IsNullOrEmpty(accountId))
                throw new ArgumentNullException(nameof(accountId));
            
            var result = await _repository.GetDeviceRegistrationsByAccountIdAsync(accountId);
            
            if(result == null)
                return new Result<IEnumerable<DeviceRegistration>, DeviceRegistrationErrorCode>(Enumerable.Empty<DeviceRegistration>());

            return new Result<IEnumerable<DeviceRegistration>, DeviceRegistrationErrorCode>(result);
        }

    }
}