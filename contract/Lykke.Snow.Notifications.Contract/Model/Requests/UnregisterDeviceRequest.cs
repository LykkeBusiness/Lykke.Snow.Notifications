namespace Lykke.Snow.Notifications.Contracts.Model.Requests
{
    public class UnregisterDeviceRequest
    {
        public string DeviceId { get; set; }
        public string ClientId { get; set; }
        
        public UnregisterDeviceRequest(string deviceId, string clientId)
        {
            this.DeviceId = deviceId;
            this.ClientId = clientId; 
        }
    }
}
