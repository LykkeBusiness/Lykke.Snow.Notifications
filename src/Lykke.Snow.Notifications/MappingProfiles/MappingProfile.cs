using System.Linq;
using AutoMapper;
using Lykke.Snow.Notifications.Client.Models;
using Lykke.Snow.Notifications.Domain.Model;
using Lykke.Snow.Notifications.Domain.Repositories;
using Lykke.Snow.Notifications.SqlRepositories.Entities;

namespace Lykke.Snow.Notifications.MappingProfiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<DeviceRegistration, DeviceRegistrationEntity>().ReverseMap();
            
            #region DeviceConfiguration

            CreateMap<DeviceConfiguration.Notification, DeviceNotificationConfigurationEntity>()
                .ForMember(x => x.NotificationType, opt => opt.MapFrom(x => x.Type));
            CreateMap<DeviceNotificationConfigurationEntity, DeviceConfiguration.Notification>()
                .ForCtorParam("type", opt => opt.MapFrom(x => x.NotificationType));
                
            CreateMap<DeviceConfigurationEntity, DeviceConfiguration>();
            CreateMap<DeviceConfiguration, DeviceConfigurationEntity>()
                .ForMember(x => x.Oid, opt => opt.Ignore())
                .ForMember(x => x.CreatedOn, opt => opt.Ignore());

            CreateMap<DeviceConfiguration, DeviceConfigurationContract>()
                .ForMember(x => x.NotificationsOn, opt => opt.MapFrom(x => x.EnabledNotifications.Select(n => n.Type)));

            CreateMap<DeviceConfigurationContract, DeviceConfiguration>()
                .ForCtorParam("notifications",
                    opt => opt.MapFrom(
                        x => x.NotificationsOn.Select(n => new DeviceConfiguration.Notification(n, true))));

            #endregion
        }
    }
}
