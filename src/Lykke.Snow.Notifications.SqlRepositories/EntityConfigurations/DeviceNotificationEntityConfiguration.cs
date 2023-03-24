using Lykke.Snow.Notifications.SqlRepositories.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lykke.Snow.Notifications.SqlRepositories.EntityConfigurations
{
    public class DeviceNotificationEntityConfiguration : IEntityTypeConfiguration<DeviceNotificationConfigurationEntity>
    {
        public void Configure(EntityTypeBuilder<DeviceNotificationConfigurationEntity> builder)
        {
            builder.HasKey(x => x.Oid);
            builder.Property(x => x.Oid).ValueGeneratedOnAdd();
            builder.HasIndex(x => new {x.DeviceConfigurationId, x.NotificationType}).IsUnique();
            
            builder.Property(x => x.NotificationType).HasMaxLength(128).IsRequired();
            builder.Property(x => x.Enabled).IsRequired();
        }
    }
}
