﻿using System;
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
    public class DeviceConfigurationRepository : IDeviceConfigurationRepository
    {
        /// <summary>
        /// The specification is used to express the unique entity constraint.
        /// Important note: it might not be implemented by particular database provider
        /// so be cautious when using it in code. Accept multiple entities having the same
        /// values for the properties specified in the specification.
        /// </summary>
        private sealed class UniqueEntitySpecification : Specification<DeviceConfigurationEntity>
        {
            public UniqueEntitySpecification(string deviceId, string accountId)
            {
                AddCriteria(x => x.DeviceId == deviceId && x.AccountId == accountId);
            }

            public UniqueEntitySpecification(DeviceConfiguration deviceConfiguration) : this(
                deviceConfiguration.DeviceId, deviceConfiguration.AccountId)
            {
            }
        }
        
        private readonly Lykke.Common.MsSql.IDbContextFactory<NotificationsDbContext> _contextFactory;
        private readonly IMapper _mapper;
        
        public DeviceConfigurationRepository(
            Lykke.Common.MsSql.IDbContextFactory<NotificationsDbContext> contextFactory,
            IMapper mapper)
        {
            _contextFactory = contextFactory;
            _mapper = mapper;
        }

        /// <summary>
        /// Gets device configuration by device id
        /// </summary>
        /// <param name="deviceId">Teh device id</param>
        /// <param name="accountId">Account id</param>
        /// <returns></returns>
        /// <exception cref="EntityNotFoundException">When there is no device configuration for device id</exception>
        public async Task<DeviceConfiguration> GetAsync(string deviceId, string accountId)
        {
            await using var context = _contextFactory.CreateDataContext();

            var entity = await context.DeviceConfigurations
                .Include(x => x.Notifications)
                .FirstOrDefaultAsync(new UniqueEntitySpecification(deviceId, accountId));
            
            return _mapper.Map<DeviceConfiguration>(entity);
        }

        /// <summary>
        /// Adds or updates device configuration
        /// </summary>
        /// <param name="deviceConfiguration"></param>
        /// <exception cref="EntityNotFoundException">
        /// When there is no device configuration for device id.
        /// Potentially possible in highly concurrent environments.
        /// </exception>
        /// <exception cref="EntityAlreadyExistsException">
        /// When there is already device configuration for device id.
        /// Potentially possible in highly concurrent environments.
        /// </exception>
        public async Task AddOrUpdateAsync(DeviceConfiguration deviceConfiguration)
        {
            await using var context = _contextFactory.CreateDataContext();

            var existingEntity = await context.DeviceConfigurations
                .Include(x => x.Notifications)
                .FirstOrDefaultAsync(new UniqueEntitySpecification(deviceConfiguration));

            if (existingEntity == null)
            {
                await TryAddAsync(context, deviceConfiguration);
                return;
            }

            await TryUpdateAsync(context, deviceConfiguration, existingEntity);
        }

        /// <summary>
        /// Removes device configuration
        /// </summary>
        /// <param name="deviceId">The device id</param>
        /// <param name="accountId">The account id</param>
        /// <exception cref="EntityNotFoundException">
        /// When there is no device configuration for device id.
        /// </exception>
        public async Task RemoveAsync(string deviceId, string accountId)
        {
            await using var context = _contextFactory.CreateDataContext();

            var entities = await context.DeviceConfigurations
                .Include(x => x.Notifications)
                .Where(new UniqueEntitySpecification(deviceId, accountId))
                .ToListAsync();

            if (entities.Count == 0)
                throw new EntityNotFoundException(deviceId);

            context.DeviceConfigurations.RemoveRange(entities);

            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException e) when (e.IsMissingDataException())
            {
                throw new EntityNotFoundException(deviceId);
            }
        }

        private async Task TryAddAsync(NotificationsDbContext context, DeviceConfiguration deviceConfiguration)
        {
            var entity = _mapper.Map<DeviceConfigurationEntity>(deviceConfiguration);
            entity.CreatedOn = entity.LastUpdated = DateTime.UtcNow;
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
            existingEntity.LastUpdated = DateTime.UtcNow;
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
