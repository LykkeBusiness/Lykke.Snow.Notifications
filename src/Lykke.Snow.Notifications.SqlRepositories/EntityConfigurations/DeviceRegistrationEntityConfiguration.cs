using Lykke.Snow.Notifications.SqlRepositories.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lykke.Snow.Notifications.SqlRepositories.EntityConfigurations
{
    public class DeviceRegistrationEntityConfiguration : IEntityTypeConfiguration<DeviceRegistrationEntity>
    {
        public void Configure(EntityTypeBuilder<DeviceRegistrationEntity> builder)
        {
            builder.HasKey(x => x.Oid);
            builder.Property(x => x.Oid).ValueGeneratedOnAdd();
            builder.HasIndex(x => x.DeviceToken).IsUnique();
            
            builder.Property(x => x.AccountId).HasMaxLength(128).IsRequired();
            builder.Property(x => x.DeviceToken).HasMaxLength(512).IsRequired();
            builder.Property(x => x.RegisteredOn).HasColumnType("datetime2").IsRequired();
            // currently we can allow having multiple registered tokens per device since retention policy
            // is not defined yet
            builder.Property(x => x.DeviceId).HasMaxLength(128).IsRequired();
        }
    }
}
