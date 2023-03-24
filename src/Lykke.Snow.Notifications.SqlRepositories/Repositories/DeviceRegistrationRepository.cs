using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Lykke.Common.MsSql;
using Lykke.Snow.Notifications.Domain.Exceptions;
using Lykke.Snow.Notifications.Domain.Model;
using Lykke.Snow.Notifications.Domain.Repositories;
using Lykke.Snow.Notifications.SqlRepositories.Entities;
using Microsoft.EntityFrameworkCore;

namespace Lykke.Snow.Notifications.SqlRepositories.Repositories
{
    public class DeviceRegistrationRepository : IDeviceRegistrationRepository
    {
        private readonly Lykke.Common.MsSql.IDbContextFactory<NotificationsDbContext> _contextFactory;
        private readonly IMapper _mapper;

        public DeviceRegistrationRepository(Lykke.Common.MsSql.IDbContextFactory<NotificationsDbContext> contextFactory,
            IMapper mapper)
        {
            _contextFactory = contextFactory;
            _mapper = mapper;
        }

        public async Task<DeviceRegistration> GetDeviceRegistrationAsync(string deviceToken)
        {
            await using var context = _contextFactory.CreateDataContext();
            var entity = await context.DeviceRegistrations.SingleOrDefaultAsync(x => x.DeviceToken == deviceToken);
            
            if (entity == null)
                throw new EntityNotFoundException(deviceToken);
        
            return _mapper.Map<DeviceRegistration>(entity);
        }

        public async Task<IReadOnlyList<DeviceRegistration>> GetDeviceRegistrationsByAccountIdAsync(string accountId)
        {
            await using var context = _contextFactory.CreateDataContext();
            
            var entitites = await context.DeviceRegistrations.Where(x => x.AccountId == accountId)
                .Select(devRegEntity => _mapper.Map<DeviceRegistration>(devRegEntity)).ToListAsync();
            
            return entitites;
        }

        public async Task AddOrUpdateAsync(DeviceRegistration deviceRegistration)
        {
            await using var context = _contextFactory.CreateDataContext();
            var existingEntity = await context.DeviceRegistrations
                .SingleOrDefaultAsync(x => x.DeviceToken == deviceRegistration.DeviceToken);
                
            if(existingEntity == null)
            {
                await TryAddAsync(context, deviceRegistration);
                return;
            }
            
            await TryUpdateAsync(context, deviceRegistration, existingEntity);
        }

        public async Task DeleteAsync(int oid)
        {
            await using var context = _contextFactory.CreateDataContext();
            var entity = new DeviceRegistrationEntity() { Oid = oid };
            
            context.Attach(entity);
            context.DeviceRegistrations.Remove(entity);
            
            try 
            {
                await context.SaveChangesAsync();
            }
            catch(DbUpdateConcurrencyException e) when (e.IsMissingDataException())
            {
                throw new EntityNotFoundException(oid);
            }
        }

        private async Task TryAddAsync(NotificationsDbContext context, DeviceRegistration deviceRegistration)
        {
            var entity = _mapper.Map<DeviceRegistrationEntity>(deviceRegistration);
            await context.DeviceRegistrations.AddAsync(entity);
            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateException e)
            {
                if (e.ValueAlreadyExistsException())
                {
                    throw new EntityAlreadyExistsException(entity: entity);
                }

                throw;
            }
        }

        private async Task TryUpdateAsync(NotificationsDbContext context,
            DeviceRegistration deviceRegistration,
            DeviceRegistrationEntity existingEntity)
        {
            _mapper.Map(deviceRegistration, existingEntity);
            context.DeviceRegistrations.Update(existingEntity);
            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException e) when (e.IsMissingDataException())
            {
                throw new EntityNotFoundException(existingEntity.Oid);
            }
        }
    }
}
