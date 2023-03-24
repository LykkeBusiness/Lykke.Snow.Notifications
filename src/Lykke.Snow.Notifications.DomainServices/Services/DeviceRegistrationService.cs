using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lykke.Snow.Common.Model;
using Lykke.Snow.FirebaseIntegration.Interfaces;
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
        private readonly IDeviceConfigurationRepository _deviceConfigurationRepository;
        private readonly IFcmIntegrationService _fcmIntegrationService;
        private readonly ISystemClock _systemClock;

        public DeviceRegistrationService(IDeviceRegistrationRepository repository,
            ISystemClock systemClock,
            IFcmIntegrationService fcmIntegrationService,
            IDeviceConfigurationRepository deviceConfigurationRepository)
        {
            _repository = repository;
            _systemClock = systemClock;
            _fcmIntegrationService = fcmIntegrationService;
            _deviceConfigurationRepository = deviceConfigurationRepository;
        }

        public async Task<Result<DeviceRegistrationErrorCode>> RegisterDeviceAsync(DeviceRegistration deviceRegistration, 
            string deviceId,
            string locale)
        {
            var isValid = await _fcmIntegrationService.IsDeviceTokenValid(deviceToken: deviceRegistration.DeviceToken);
            
            if(!isValid)
                return new Result<DeviceRegistrationErrorCode>(DeviceRegistrationErrorCode.DeviceTokenNotValid);
            
            try
            {
                await _repository.InsertAsync(deviceRegistration);
                
                await _deviceConfigurationRepository.AddOrUpdateAsync(
                    DeviceConfiguration.Default(deviceId, deviceRegistration.AccountId, locale));

                return new Result<DeviceRegistrationErrorCode>();
            }
            catch(EntityAlreadyExistsException)
            {
                return new Result<DeviceRegistrationErrorCode>(DeviceRegistrationErrorCode.AlreadyRegistered);
            }
        }

        public async Task<Result<DeviceRegistrationErrorCode>> UnregisterDeviceAsync(string deviceToken)
        {
            DeviceRegistration? result = null;
            try
            {
                result = await _repository.GetDeviceRegistrationAsync(deviceToken: deviceToken);
            }
            catch (EntityNotFoundException)
            {
                return new Result<DeviceRegistrationErrorCode>(DeviceRegistrationErrorCode.DoesNotExist);
            }

            if (result == null)
            {
                return new Result<DeviceRegistrationErrorCode>(DeviceRegistrationErrorCode.DoesNotExist);
            }

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
