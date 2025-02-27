using System.Collections.Generic;
using Lykke.Snow.Notifications.Domain.Enums;
using Meteor.Client.Models;

namespace Lykke.Snow.Notifications.DomainServices.Mapping
{
    internal static class MeteorMessageMapping
    {
        internal static readonly IReadOnlyDictionary<MessageEventType, NotificationType> NotificationTypeMapping = new Dictionary<MessageEventType, NotificationType>()
        {
            { MessageEventType.Custom, NotificationType.InboxMessage },
#pragma warning disable CS0618
            { MessageEventType.MarketHoliday, NotificationType.MarketHoliday },
            { MessageEventType.PlatformHoliday, NotificationType.PlatformHoliday },
#pragma warning restore CS0618
            { MessageEventType.PositionsAboutToClose, NotificationType.CAPositionAboutToClose },
            { MessageEventType.PositionsAboutToClose_871m, NotificationType.CAPositionAboutToClose }
        };
    }
}
