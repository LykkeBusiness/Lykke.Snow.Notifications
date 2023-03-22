using System.Threading.Tasks;
using AutoMapper;
using Lykke.Common.MsSql;
using Lykke.Snow.Notifications.Domain.Repositories;
using Lykke.Snow.Notifications.SqlRepositories.Entities;
using Lykke.Snow.Notifications.SqlRepositories.Exceptions;
using Microsoft.EntityFrameworkCore;
using DeviceConfiguration = Lykke.Snow.Notifications.Domain.Repositories.DeviceConfigurationStub;

namespace Lykke.Snow.Notifications.SqlRepositories.Repositories
{
    public class DeviceConfigurationRepository : IDeviceConfigurationRepository
    {
        private readonly Lykke.Common.MsSql.IDbContextFactory<NotificationsDbContext> _contextFactory;
        private readonly IMapper _mapper;

        public DeviceConfigurationRepository(
            Lykke.Common.MsSql.IDbContextFactory<NotificationsDbContext> contextFactory,
            IMapper mapper)
        {
            _contextFactory = contextFactory;
            _mapper = mapper;
        }

        public async Task<DeviceConfiguration?> GetByIdAsync(int id)
        {
            await using var context = _contextFactory.CreateDataContext();

            var entity = await context.DeviceConfigurations
                .Include(x => x.Notifications)
                .FirstOrDefaultAsync(x => x.Oid == id);
            
            if (entity == null)
                throw new EntityNotFoundException(id);
            
            return _mapper.Map<DeviceConfiguration>(entity);
        }

        public async Task AddAsync(DeviceConfiguration deviceConfiguration)
        {
            await using var context = _contextFactory.CreateDataContext();
            
            var entity = _mapper.Map<DeviceConfigurationEntity>(deviceConfiguration);
            await context.DeviceConfigurations.AddAsync(entity);
            
            try
            {
                await context.SaveChangesAsync();
            }
            catch(DbUpdateException e)
            {
                if(e.ValueAlreadyExistsException())
                {
                    throw new EntityAlreadyExistsException(entity: entity);
                }
                
                throw;
            };
        }

        public async Task UpdateAsync(DeviceConfiguration deviceConfiguration)
        {
            await using var context = _contextFactory.CreateDataContext();
            
            var entity = _mapper.Map<DeviceConfigurationEntity>(deviceConfiguration);
            context.DeviceConfigurations.Update(entity);

            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException e) when (e.IsMissingDataException())
            {
                throw new EntityNotFoundException(entity.Oid);
            }
        }

        public async Task RemoveAsync(int id)
        {
            await using var context = _contextFactory.CreateDataContext();
            
            var entity = new DeviceConfigurationEntity {Oid = id};
            context.DeviceConfigurations.Attach(entity);
            context.DeviceConfigurations.Remove(entity);
            
            try 
            {
                await context.SaveChangesAsync();
            }
            catch(DbUpdateConcurrencyException e) when (e.IsMissingDataException())
            {
                throw new EntityNotFoundException(id);
            }
        }
    }
}
