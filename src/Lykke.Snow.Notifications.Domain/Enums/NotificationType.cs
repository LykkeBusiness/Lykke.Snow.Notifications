namespace Lykke.Snow.Notifications.Domain.Enums
{
    public enum NotificationType
    {
        NotSpecified,
        AccountLocked,
        AccountUnlocked,
        CashLocked,
        CashUnlocked,
        DepositSucceeded,
        InboxMessage,
        Liquidation,
        MarginCall1,
        MarginCall2,
        OrderExecuted,
        OrderExpired,
        /// <summary>
        /// Introduced to establish parent-child relationship 
        /// for the all on-behalf notification types.
        /// 'OnBehalfAction' must be used to enable/disable all on-behalf notifications.
        /// For localization (composing body and title) - individual on-behalf notifications is being used.
        /// This 'OnBehalfAction' is only for configuration purposes.
        /// </summary>
        OnBehalfAction,
        PositionClosed,
        WithdrawalSucceeded,
        PriceAlertTriggered,
        OnBehalfOrderPlacement,
        OnBehalfOrderModification,
        OnBehalfOrderCancellation,
        OnBehalfPositionClosing,
        CAPositionAboutToClose,
        MarketHoliday,
        PlatformHoliday,
        OnBehalfOrderExecution
    }
}
