using System;
using System.Collections.Generic;
using MessagePack;

namespace Lykke.Snow.Notifications.Subscribers.Messages
{
    public enum MessageEventType
    {
        Custom,
        MarketHoliday,
        PlatformHoliday,
        PositionsAboutToClose,
        PriceAlertTriggered,
        PositionsAboutToClose_871m,
    }

    [MessagePackObject]
    public class MessagePreviewEvent
    {
        [Key(0)]
        public string? Id { get; set; }

        [Key(1)]
        public string? SenderId { get; set; }

        [Key(2)]
        public string? Subject { get; set; }

        [Key(3)]
        public bool IsImportant { get; set; }

        [Key(4)]
        public DateTime Timestamp { get; set; }

        [Key(5)]
        public bool RequiresPopup { get; set; }

        [Key(6)]
        public IEnumerable<string>? Recipients { get; set; }

        [Key(7)]
        public bool SendToAllInvestors { get; set; }

        [Key(8)]
        public bool SendToInvestorsWithActiveOrders { get; set; }

        [Key(9)]
        public IEnumerable<string>? ActiveOrderAssetIds { get; set; }

        [Key(10)]
        public bool SendToInvestorsWithOpenPositions { get; set; }

        [Key(11)]
        public IEnumerable<string>? OpenPositionAssetIds { get; set; }

        [Key(12)]
        public bool IsAndClauseApplied { get; set; }

        /// <summary>
        /// Gets or sets the OperationId.
        /// This field is used by Meteor for message deduplication.
        /// </summary>
        [Key(13)]
        public string? OperationId { get; set; }

        [Key(14)]
        public bool IsRead { get; set; }

        [Key(15)]
        public MessageEventType Event { get; set; }

        [Key(16)]
        public string[]? LocalizationAttributes { get; set; }

        [Key(17)]
        public DateTime? ExpirationTimestamp { get; set; }

        [Key(18)]
        public bool RequiresAcceptance { get; set; }

        [Key(19)]
        public string? SenderName { get; set; }

        [Key(20)]
        public string? Content { get; set; }
    }
}
