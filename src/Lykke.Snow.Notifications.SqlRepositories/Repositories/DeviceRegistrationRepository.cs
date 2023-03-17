using System.Threading.Tasks;
using AutoMapper;
using Lykke.Common.MsSql;
using Lykke.Snow.Common.Model;
using Lykke.Snow.Notifications.Domain.Enums;
using Lykke.Snow.Notifications.Domain.Model;
using Lykke.Snow.Notifications.Domain.Repositories;
using Lykke.Snow.Notifications.SqlRepositories.Entities;
using Microsoft.EntityFrameworkCore;

namespace Lykke.Snow.Notifications.SqlRepositories.Repositories
{
    public class DeviceRegistrationRepository : IDeviceRegistrationRepository
    {
        private readonly MsSqlContextFactory<NotificationsDbContext> _contextFactory;
        private readonly IMapper _mapper;

        public DeviceRegistrationRepository(MsSqlContextFactory<NotificationsDbContext> contextFactory, IMapper mapper)
        {
            _contextFactory = contextFactory;
            _mapper = mapper;
        }

        public async Task<Result<DeviceRegistration, DeviceRegistrationErrorCode>> GetDeviceRegistrationAsync(string deviceToken)
        {
            await using var context = _contextFactory.CreateDataContext();
            var entity = await context.DeviceRegistrations.SingleOrDefaultAsync(x => x.DeviceToken == deviceToken);
            
            if (entity == null)
                return new Result<DeviceRegistration, DeviceRegistrationErrorCode>(DeviceRegistrationErrorCode.DoesNotExist);
        
            return new Result<DeviceRegistration, DeviceRegistrationErrorCode>(_mapper.Map<DeviceRegistration>(entity));
        }

        public async Task<Result<DeviceRegistrationErrorCode>> InsertAsync(DeviceRegistration deviceRegistration)
        {
            await using var context = _contextFactory.CreateDataContext();
            var entity = _mapper.Map<DeviceRegistrationEntity>(deviceRegistration);
            
            await context.DeviceRegistrations.AddAsync(entity);
            
            try
            {
                await context.SaveChangesAsync();
                return new Result<DeviceRegistrationErrorCode>();
            }
            catch(DbUpdateException e)
            {
                if(e.ValueAlreadyExistsException())
                {
                    return new Result<DeviceRegistrationErrorCode>(DeviceRegistrationErrorCode.AlreadyRegistered);
                }
                
                throw;
            };
        }

        public async Task<Result<DeviceRegistrationErrorCode>> DeleteAsync(string deviceToken)
        {
            await using var context = _contextFactory.CreateDataContext();
            var entity = new DeviceRegistrationEntity() { DeviceToken = deviceToken };
            
            context.Attach(entity);
            context.DeviceRegistrations.Remove(entity);
            
            try 
            {
                await context.SaveChangesAsync();
                return new Result<DeviceRegistrationErrorCode>();
            }
            catch(DbUpdateConcurrencyException e) when (e.IsMissingDataException())
            {
                return new Result<DeviceRegistrationErrorCode>(DeviceRegistrationErrorCode.DoesNotExist);
            }
        }
    }
}
