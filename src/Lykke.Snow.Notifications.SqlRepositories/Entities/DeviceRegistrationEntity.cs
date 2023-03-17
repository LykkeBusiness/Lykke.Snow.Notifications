using System;

namespace Lykke.Snow.Notifications.SqlRepositories.Entities
{
    //TODO: create a unique constraint clientId-deviceToken
    //also consider removing Id, why would we need that?


    //Device token also unique
    public class DeviceRegistrationEntity
    {
        public string ClientId { get; set; }
        public string DeviceToken { get; set; }
        public DateTime RegisteredOn { get; set; }
    }
}
