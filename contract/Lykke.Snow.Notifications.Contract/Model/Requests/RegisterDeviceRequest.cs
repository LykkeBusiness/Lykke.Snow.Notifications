namespace Lykke.Snow.Notifications.Contracts.Model.Requests
{
    public class RegisterDeviceRequest
    {
        public string ClientId { get; set; }
        public string DeviceToken { get; set; }
    }
}
