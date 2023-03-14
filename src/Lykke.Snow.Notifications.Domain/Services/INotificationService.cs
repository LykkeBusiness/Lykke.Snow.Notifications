using System;
using System.Threading.Tasks;
using Lykke.Snow.Notifications.Domain.Model;

namespace Lykke.Snow.Notifications.Domain.Services
{
    public interface INotificationService
    {
        bool IsInitialized { get; }
        void Initialize(string credentialsFilePath);
        Task SendNotification(NotificationMessageBase message);
    }
}
