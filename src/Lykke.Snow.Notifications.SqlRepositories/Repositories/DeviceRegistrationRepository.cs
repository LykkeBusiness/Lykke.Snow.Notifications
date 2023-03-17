using System.Threading.Tasks;
using Lykke.Common.MsSql;
using Lykke.Snow.Common.Model;
using Lykke.Snow.Notifications.Domain.Enums;
using Lykke.Snow.Notifications.Domain.Model;
using Lykke.Snow.Notifications.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Lykke.Snow.Notifications.SqlRepositories.Repositories
{
    public class DeviceRegistrationRepository : IDeviceRegistrationRepository
    {
        private readonly MsSqlContextFactory<NotificationsDbContext> _contextFactory;

        public DeviceRegistrationRepository(MsSqlContextFactory<NotificationsDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<Result<DeviceRegistration, DeviceRegistrationErrorCode>> GetDeviceRegistrationAsync(string deviceToken)
        {
            await using var context = _contextFactory.CreateDataContext();
            var entity = await context.DeviceRegistrations.SingleOrDefaultAsync(x => x.DeviceToken == deviceToken);
            
            if (entity == null)
                return new Result<DeviceRegistration, DeviceRegistrationErrorCode>(DeviceRegistrationErrorCode.DoesNotExist);
        }

        public Task<Result<DeviceRegistrationErrorCode>> InsertAsync(string deviceToken)
        {
            throw new System.NotImplementedException();
        }

        public Task<Result<DeviceRegistrationErrorCode>> DeleteAsync(string deviceToken, string clientId)
        {
            throw new System.NotImplementedException();
        }
    }
}
