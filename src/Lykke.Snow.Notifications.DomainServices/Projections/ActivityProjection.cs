using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.MarginTrading.Activities.Contracts.Models;
using Lykke.Snow.Notifications.Domain.Services;

namespace Lykke.Snow.Notifications.DomainServices.Projections
{
    public class ActivityProjection
    {
        private readonly IActivityHandler _activityHandler;

        public ActivityProjection(IActivityHandler activityHandler)
        {
            _activityHandler = activityHandler;
        }

        [UsedImplicitly]
        public async Task Handle(ActivityEvent e)
        {
            await _activityHandler.Handle(e);
        }
    }
}
