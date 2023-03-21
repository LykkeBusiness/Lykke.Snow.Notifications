using AutoMapper;
using Lykke.Snow.Notifications.Domain.Model;
using Lykke.Snow.Notifications.SqlRepositories.Entities;

namespace Lykke.Snow.Notifications.MappingProfiles
{
    public class DeviceRegistrationMappingProfile : Profile
    {
        public DeviceRegistrationMappingProfile()
        {
            CreateMap<DeviceRegistration, DeviceRegistrationEntity>().ReverseMap();
        }
    }
}