﻿using Lykke.Snow.Notifications.SqlRepositories.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lykke.Snow.Notifications.SqlRepositories.EntityConfigurations
{
    public class DeviceEntityConfiguration : IEntityTypeConfiguration<DeviceConfigurationEntity>
    {
        public void Configure(EntityTypeBuilder<DeviceConfigurationEntity> builder)
        {
            builder.HasKey(x => x.Oid);
            builder.Property(x => x.Oid).ValueGeneratedOnAdd();
            builder.HasIndex(x => x.DeviceId).IsUnique();
            
            builder.Property(x => x.AccountId).HasMaxLength(128).IsRequired();
            builder.Property(x => x.CreatedOn).HasColumnType("datetime2").IsRequired();
            builder
                .HasMany(x => x.Notifications)
                .WithOne(x => x.DeviceConfiguration)
                .HasForeignKey(x => x.DeviceConfigurationId);
        }
    }
}
