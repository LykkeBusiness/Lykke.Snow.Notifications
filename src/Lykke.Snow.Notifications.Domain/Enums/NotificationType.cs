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
        OnBehalfAction,
        PositionClosed,
        WithdrawalSucceeded,
        OnBehalfOrderPlacement,
        OnBehalfOrderModification,
        OnBehalfOrderCancellation,
        OnBehalfPositionClosing,
    }
}
