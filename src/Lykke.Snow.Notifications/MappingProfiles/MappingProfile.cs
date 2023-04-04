using System;
using System.Linq;
using AutoMapper;
using Lykke.Snow.Notifications.Client.Model.Requests;
using Lykke.Snow.Notifications.Client.Models;
using Lykke.Snow.Notifications.Domain.Model;
using Lykke.Snow.Notifications.SqlRepositories.Entities;

namespace Lykke.Snow.Notifications.MappingProfiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<DeviceRegistration, DeviceRegistrationEntity>()
                .ForMember(dest => dest.Oid, opt => opt.Ignore())
                .ReverseMap();

            CreateMap<RegisterDeviceRequest, DeviceRegistration>()
                .ConstructUsing((ctx) => new DeviceRegistration(
                    ctx.AccountId,
                    ctx.DeviceToken,
                    ctx.DeviceId,
                    DateTime.UtcNow
                ));
            
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
                .ForMember(x => x.NotificationsOn, opt => opt.MapFrom(x => x.EnabledNotificationTypes));

            CreateMap<DeviceConfigurationContract, DeviceConfiguration>()
                .ForCtorParam("notifications",
                    opt => opt.MapFrom(
                        x => x.NotificationsOn.Select(n => new DeviceConfiguration.Notification(n, true))));

            #endregion
        }
    }
}
