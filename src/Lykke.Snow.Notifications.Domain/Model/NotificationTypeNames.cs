namespace Lykke.Snow.Notifications.Domain.Model
{
    public class NotificationTypeNames
    {
        public const string AccountLockedNotification = nameof(Notifications.Domain.Model.NotificationTypes.AccountLockedNotification);
        public const string AccountUnlockedNotification = nameof(Notifications.Domain.Model.NotificationTypes.AccountUnlockedNotification);
        public const string CashLockedNotification = nameof(Notifications.Domain.Model.NotificationTypes.CashLockedNotification);
        public const string CashUnlockedNotification = nameof(Notifications.Domain.Model.NotificationTypes.CashUnlockedNotification);
        public const string DepositSucceededNotification = nameof(Notifications.Domain.Model.NotificationTypes.DepositSucceededNotification);
        public const string DepositFailedNotification = nameof(Notifications.Domain.Model.NotificationTypes.DepositFailedNotification);
        public const string InboxMessageNotification = nameof(Notifications.Domain.Model.NotificationTypes.InboxMessageNotification);
        public const string MarginCall1Notification = nameof(Notifications.Domain.Model.NotificationTypes.MarginCall1Notification);
        public const string MarginCall2Notification = nameof(Notifications.Domain.Model.NotificationTypes.MarginCall2Notification);
        public const string OnBehalfActionNotification = nameof(Notifications.Domain.Model.NotificationTypes.OnBehalfActionNotification);
        public const string OrderExecutedNotification = nameof(Notifications.Domain.Model.NotificationTypes.OrderExecutedNotification);
        public const string OrderExpiredNotification = nameof(Notifications.Domain.Model.NotificationTypes.OrderExpiredNotification);
        public const string PositionClosedNotification = nameof(Notifications.Domain.Model.NotificationTypes.PositionClosedNotification);
        public const string WithdrawalSucceededNotification = nameof(Notifications.Domain.Model.NotificationTypes.WithdrawalSucceededNotification);
        public const string WithdrawalFailedNotification = nameof(Notifications.Domain.Model.NotificationTypes.WithdrawalFailedNotification);
    }
}
