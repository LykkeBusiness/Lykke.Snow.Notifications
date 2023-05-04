using System.Collections.Generic;
using Lykke.MarginTrading.Activities.Contracts.Models;
using Lykke.Snow.Notifications.Domain.Enums;
using Meteor.Client.Models;

namespace Lykke.Snow.Notifications.DomainServices.Mapping
{
    internal static class MeteorMessageMapping
    {
        internal static readonly IReadOnlyDictionary<MessageEventType, NotificationType> NotificationTypeMapping = new Dictionary<MessageEventType, NotificationType>()
        {
            { MessageEventType.Custom, NotificationType.InboxMessage },
            { MessageEventType.MarketHoliday, NotificationType.MarketHoliday },
            { MessageEventType.PlatformHoliday, NotificationType.PlatformHoliday },
            { MessageEventType.PositionsAboutToClose, NotificationType.CAPositionAboutToClose },
            { MessageEventType.PositionsAboutToClose_871m, NotificationType.CAPositionAboutToClose }
        };
    }
}
