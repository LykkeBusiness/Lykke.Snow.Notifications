using System;
using System.Collections.Generic;
using System.Linq;
using Lykke.MarginTrading.Activities.Contracts.Models;
using Lykke.Snow.Notifications.Domain.Enums;

namespace Lykke.Snow.Notifications.DomainServices.Mapping
{
    internal static class ActivityTypeMapping
    {
        internal static readonly IReadOnlyDictionary<ActivityTypeContract, NotificationType> NotificationTypeMapping = new Dictionary<ActivityTypeContract, NotificationType>()
        {
            { ActivityTypeContract.AccountTradingDisabled, NotificationType.AccountLocked },
            { ActivityTypeContract.AccountTradingEnabled, NotificationType.AccountUnlocked },
            { ActivityTypeContract.AccountDepositSucceeded, NotificationType.DepositSucceeded },
            { ActivityTypeContract.AccountWithdrawalSucceeded, NotificationType.WithdrawalSucceeded },
            { ActivityTypeContract.AccountWithdrawalEnabled, NotificationType.CashUnlocked },
            { ActivityTypeContract.AccountWithdrawalDisabled, NotificationType.CashLocked },
            { ActivityTypeContract.Liquidation, NotificationType.Liquidation },
            { ActivityTypeContract.MarginCall1, NotificationType.MarginCall1 },
            { ActivityTypeContract.MarginCall2, NotificationType.MarginCall2 },
            { ActivityTypeContract.OrderExecution, NotificationType.OrderExecuted },
            { ActivityTypeContract.OrderAcceptanceAndExecution, NotificationType.OrderExecuted },
            { ActivityTypeContract.OrderExpiry, NotificationType.OrderExpired },
            { ActivityTypeContract.PositionClosing, NotificationType.PositionClosed },
            { ActivityTypeContract.PositionPartialClosing, NotificationType.PositionClosed }
        };

        internal static readonly IReadOnlyDictionary<ActivityTypeContract, Func<ActivityEvent, string[]>> DescriptionEnrichments =
            new Dictionary<ActivityTypeContract, Func<ActivityEvent, string[]>>
        {
            {ActivityTypeContract.AccountWithdrawalSucceeded, (e) => { return e.Activity.DescriptionAttributes.ToList().Append(e.Activity.AccountId).ToArray(); }},
            {ActivityTypeContract.AccountDepositSucceeded, (e) => { return e.Activity.DescriptionAttributes.ToList().Append(e.Activity.AccountId).ToArray(); }},
            {ActivityTypeContract.AccountWithdrawalEnabled, (e) => { return e.Activity.DescriptionAttributes.ToList().Append(e.Activity.AccountId).ToArray(); }},
            {ActivityTypeContract.AccountWithdrawalDisabled, (e) => { return e.Activity.DescriptionAttributes.ToList().Append(e.Activity.AccountId).ToArray(); }},
            {ActivityTypeContract.AccountTradingEnabled, (e) => { return e.Activity.DescriptionAttributes.ToList().Append(e.Activity.AccountId).ToArray(); }},
            {ActivityTypeContract.AccountTradingDisabled, (e) => { return e.Activity.DescriptionAttributes.ToList().Append(e.Activity.AccountId).ToArray(); }}
        };
    }
}
