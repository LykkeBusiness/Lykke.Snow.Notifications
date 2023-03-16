namespace Lykke.Snow.Notifications.Contracts.Model.Requests
{
    public class RegisterDeviceRequest
    {
        public string ClientId { get; set; }
        public string DeviceToken { get; set; }
        
        public RegisterDeviceRequest(string clientId, string deviceToken)
        {
            ClientId = clientId;
            DeviceToken = deviceToken;
        }
    }
}
