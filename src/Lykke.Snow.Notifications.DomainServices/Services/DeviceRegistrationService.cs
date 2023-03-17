using System.Threading.Tasks;
using Lykke.Snow.Common.Model;
using Lykke.Snow.Notifications.Domain.Enums;
using Lykke.Snow.Notifications.Domain.Model;
using Lykke.Snow.Notifications.Domain.Repositories;
using Lykke.Snow.Notifications.Domain.Services;

namespace Lykke.Snow.Notifications.DomainServices.Services
{
    public class DeviceRegistrationService : IDeviceRegistrationService
    {
        private readonly IDeviceRegistrationRepository _repository;

        public DeviceRegistrationService(IDeviceRegistrationRepository repository)
        {
            _repository = repository;
        }

        public Task<Result<DeviceRegistrationErrorCode>> RegisterDeviceAsync(DeviceRegistration deviceRegistration)
        {
            throw new System.NotImplementedException();
        }

        public Task<Result<DeviceRegistrationErrorCode>> UnregisterDeviceAsync(DeviceRegistration deviceRegistration)
        {
            throw new System.NotImplementedException();
        }
    }
}
