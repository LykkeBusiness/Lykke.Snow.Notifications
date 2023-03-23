﻿using System.Threading.Tasks;
using AutoMapper;
using Lykke.Common.MsSql;
using Lykke.Snow.Notifications.Domain.Repositories;
using Lykke.Snow.Notifications.SqlRepositories.Entities;
using Lykke.Snow.Notifications.SqlRepositories.Exceptions;
using Microsoft.EntityFrameworkCore;

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

        public async Task<DeviceConfiguration?> GetAsync(string deviceId)
        {
            await using var context = _contextFactory.CreateDataContext();

            var entity = await context.DeviceConfigurations
                .Include(x => x.Notifications)
                .FirstOrDefaultAsync(x => x.DeviceId == deviceId);
            
            if (entity == null)
                throw new EntityNotFoundException(deviceId);

            return _mapper.Map<DeviceConfiguration>(entity);
        }

        public async Task AddOrUpdateAsync(DeviceConfiguration deviceConfiguration)
        {
            await using var context = _contextFactory.CreateDataContext();

            var existingEntity = await context.DeviceConfigurations
                .Include(x => x.Notifications)
                .FirstOrDefaultAsync(x => x.DeviceId == deviceConfiguration.DeviceId);

            if (existingEntity == null)
            {
                await TryAddAsync(context, deviceConfiguration);
                return;
            }

            await TryUpdateAsync(context, deviceConfiguration, existingEntity);
        }

        public async Task RemoveAsync(string deviceId)
        {
            await using var context = _contextFactory.CreateDataContext();
            
            var entity = await context.DeviceConfigurations
                .Include(x => x.Notifications)
                .FirstOrDefaultAsync(x => x.DeviceId == deviceId);

            if (entity == null)
                throw new EntityNotFoundException(deviceId);

            context.DeviceConfigurations.Remove(entity);
            
            try 
            {
                await context.SaveChangesAsync();
            }
            catch(DbUpdateConcurrencyException e) when (e.IsMissingDataException())
            {
                throw new EntityNotFoundException(deviceId);
            }
        }

        private async Task TryAddAsync(NotificationsDbContext context, DeviceConfiguration deviceConfiguration)
        {
            var entity = _mapper.Map<DeviceConfigurationEntity>(deviceConfiguration);
            await context.DeviceConfigurations.AddAsync(entity);
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
            DeviceConfiguration deviceConfiguration,
            DeviceConfigurationEntity existingEntity)
        {
            _mapper.Map(deviceConfiguration, existingEntity);
            context.DeviceConfigurations.Update(existingEntity);
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