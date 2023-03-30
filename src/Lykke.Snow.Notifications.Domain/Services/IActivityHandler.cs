using System.Threading.Tasks;
using Lykke.MarginTrading.Activities.Contracts.Models;

namespace Lykke.Snow.Notifications.Domain.Services
{
    public interface IActivityHandler
    {
        Task Handle(ActivityEvent e);
    }
}
