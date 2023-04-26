using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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
        private readonly ConcurrentDictionary<(string accountId, string deviceToken), SemaphoreSlim> _locks =
            new ConcurrentDictionary<(string accountId, string deviceToken), SemaphoreSlim>();

        public DeviceRegistrationRepository(Lykke.Common.MsSql.IDbContextFactory<NotificationsDbContext> contextFactory,
            IMapper mapper)
        {
            _contextFactory = contextFactory;
            _mapper = mapper;
        }


        public async Task<IReadOnlyList<DeviceRegistration>> GetDeviceRegistrationsAsync(string deviceToken)
        {
            await using var context = _contextFactory.CreateDataContext();
            var entities = await context.DeviceRegistrations.Where(x => x.DeviceToken == deviceToken)
                .Select(e => _mapper.Map<DeviceRegistration>(e)).ToListAsync();
            
            return entities;
        }

        public async Task<IReadOnlyList<DeviceRegistration>> GetDeviceRegistrationsByAccountIdAsync(string accountId)
        {
            await using var context = _contextFactory.CreateDataContext();
            
            var entitites = await context.DeviceRegistrations.Where(x => x.AccountId == accountId)
                .Select(devRegEntity => _mapper.Map<DeviceRegistration>(devRegEntity)).ToListAsync();
            
            return entitites;
        }

        public async Task<IReadOnlyList<DeviceRegistration>> GetDeviceRegistrationsByAccountIdsAsync(string[] accountIds)
        {
            await using var context = _contextFactory.CreateDataContext();

            var entitites = await context.DeviceRegistrations.Where(x => accountIds.Any(id => id == x.AccountId))
                .Select(devRegEntity => _mapper.Map<DeviceRegistration>(devRegEntity)).ToListAsync();
            
            return entitites;
        }

        public async Task AddOrUpdateAsync(DeviceRegistration model)
        {
            var lockObject = _locks.GetOrAdd(
                (model.AccountId, model.DeviceToken),
                _ => new SemaphoreSlim(1, 1));

            await lockObject.WaitAsync();

            try
            {
                await using var context = _contextFactory.CreateDataContext();
                var existingEntity = await context.DeviceRegistrations
                    .SingleOrDefaultAsync(x =>
                        x.DeviceToken == model.DeviceToken && x.AccountId == model.AccountId);

                if (existingEntity == null)
                {
                    await TryAddAsync(context, model);
                    return;
                }

                await TryUpdateAsync(context, model, existingEntity);
            }
            finally
            {
                lockObject.Release();
            }
        }

        public async Task RemoveAsync(int oid)
        {
            await using var context = _contextFactory.CreateDataContext();
            var entity = new DeviceRegistrationEntity() { Oid = oid };

            context.Attach(entity);
            context.DeviceRegistrations.Remove(entity);

            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException e) when (e.IsMissingDataException())
            {
                throw new EntityNotFoundException(oid);
            }
        }

        public async Task RemoveAllAsync(int[] oids)
        {
            await using var context = _contextFactory.CreateDataContext();
            var entities = oids.Select(x => new DeviceRegistrationEntity() { Oid = x }).ToList();

            context.AttachRange(entities);
            context.DeviceRegistrations.RemoveRange(entities);

            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException e) when (e.IsMissingDataException())
            {
                throw new EntityNotFoundException(oids);
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
