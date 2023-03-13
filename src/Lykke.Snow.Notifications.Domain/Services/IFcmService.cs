using System.Threading.Tasks;

namespace Lykke.Snow.Notifications.Domain.Services
{
    public interface IFcmService
    {
        bool IsInitialized { get; }
        Task InitializeFcmClient();
        Task SendNotification();
    }
}
