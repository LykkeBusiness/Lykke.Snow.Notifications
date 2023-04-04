using System.Threading.Tasks;
using Lykke.MarginTrading.Activities.Contracts.Models;

namespace Lykke.Snow.Notifications.Domain.Services
{
    /// <summary>
    /// Responsible for handling all activity events that coming to the Notification Service
    /// </summary>
    public interface IActivityHandler
    {
        Task Handle(ActivityEvent e);
    }
}
