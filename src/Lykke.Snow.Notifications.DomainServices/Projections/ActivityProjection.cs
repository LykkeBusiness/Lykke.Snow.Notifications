using Common;
using JetBrains.Annotations;
using Lykke.MarginTrading.Activities.Contracts.Models;
using Microsoft.Extensions.Logging;

namespace Lykke.Snow.Notifications.DomainServices.Projections
{
    public class ActivityProjection
    {
        private readonly ILogger<ActivityProjection> _logger;

        public ActivityProjection(ILogger<ActivityProjection> logger)
        {
            _logger = logger;
        }
        
        [UsedImplicitly]
        public void Handle(ActivityEvent e)
        {
            //TODO: Handle the event and send corresponding push notifications
            _logger.LogInformation("A new activity event has just arrived {ActivityEvent}", e.ToJson());
        }
    }
}
