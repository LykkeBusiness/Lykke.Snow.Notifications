using System;
using Lykke.Snow.Notifications.SqlRepositories.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lykke.Snow.Notifications.SqlRepositories.EntityConfigurations
{
    public class DeviceRegistrationEntityConfiguration : IEntityTypeConfiguration<DeviceRegistrationEntity>
    {
        public void Configure(EntityTypeBuilder<DeviceRegistrationEntity> builder)
        {
            builder.HasKey(x => x.DeviceToken);
            
            builder.Property(x => x.ClientId).HasMaxLength(128).IsRequired();
            builder.Property(x => x.DeviceToken).HasMaxLength(512).IsRequired();
            builder.Property(x => x.RegisteredOn).HasColumnType("datetime").IsRequired();
        }
    }
}
