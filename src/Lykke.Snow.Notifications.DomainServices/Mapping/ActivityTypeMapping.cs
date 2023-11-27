using System;
using System.Collections.Generic;
using Lykke.MarginTrading.Activities.Contracts.Models;
using Lykke.Snow.Notifications.Domain.Enums;

namespace Lykke.Snow.Notifications.DomainServices.Mapping
{
    internal static class ActivityTypeMapping
    {
        internal static readonly IReadOnlyDictionary<Tuple<ActivityTypeContract, OnBehalf>, NotificationType> NotificationTypeMapping = new Dictionary<Tuple<ActivityTypeContract, OnBehalf>, NotificationType>()
        {
            { new Tuple<ActivityTypeContract, OnBehalf>(ActivityTypeContract.AccountTradingDisabled, OnBehalf.No), NotificationType.AccountLocked },
            { new Tuple<ActivityTypeContract, OnBehalf>(ActivityTypeContract.AccountTradingEnabled, OnBehalf.No), NotificationType.AccountUnlocked },
            { new Tuple<ActivityTypeContract, OnBehalf>(ActivityTypeContract.AccountDepositSucceeded, OnBehalf.No), NotificationType.DepositSucceeded },
            { new Tuple<ActivityTypeContract, OnBehalf>(ActivityTypeContract.AccountWithdrawalSucceeded, OnBehalf.No), NotificationType.WithdrawalSucceeded },
            { new Tuple<ActivityTypeContract, OnBehalf>(ActivityTypeContract.AccountWithdrawalEnabled, OnBehalf.No), NotificationType.CashUnlocked },
            { new Tuple<ActivityTypeContract, OnBehalf>(ActivityTypeContract.AccountWithdrawalDisabled, OnBehalf.No), NotificationType.CashLocked },
            { new Tuple<ActivityTypeContract, OnBehalf>(ActivityTypeContract.Liquidation, OnBehalf.No), NotificationType.Liquidation },
            { new Tuple<ActivityTypeContract, OnBehalf>(ActivityTypeContract.MarginCall1, OnBehalf.No), NotificationType.MarginCall1 },
            { new Tuple<ActivityTypeContract, OnBehalf>(ActivityTypeContract.MarginCall2, OnBehalf.No), NotificationType.MarginCall2 },
            { new Tuple<ActivityTypeContract, OnBehalf>(ActivityTypeContract.OrderExecution, OnBehalf.No), NotificationType.OrderExecuted },
            { new Tuple<ActivityTypeContract, OnBehalf>(ActivityTypeContract.OrderAcceptanceAndExecution, OnBehalf.No), NotificationType.OrderExecuted },
            { new Tuple<ActivityTypeContract, OnBehalf>(ActivityTypeContract.OrderExpiry, OnBehalf.No), NotificationType.OrderExpired },
            { new Tuple<ActivityTypeContract, OnBehalf>(ActivityTypeContract.PositionClosing, OnBehalf.No), NotificationType.PositionClosed },
            { new Tuple<ActivityTypeContract, OnBehalf>(ActivityTypeContract.PositionPartialClosing, OnBehalf.No), NotificationType.PositionClosed },
            { new Tuple<ActivityTypeContract, OnBehalf>(ActivityTypeContract.OrderAcceptanceAndActivation, OnBehalf.Yes), NotificationType.OnBehalfOrderPlacement },
            { new Tuple<ActivityTypeContract, OnBehalf>(ActivityTypeContract.OrderAcceptanceAndExecution, OnBehalf.Yes), NotificationType.OnBehalfOrderExecution },
            { new Tuple<ActivityTypeContract, OnBehalf>(ActivityTypeContract.OrderModification, OnBehalf.Yes), NotificationType.OnBehalfOrderModification },
            { new Tuple<ActivityTypeContract, OnBehalf>(ActivityTypeContract.OrderModificationForceOpen, OnBehalf.Yes), NotificationType.OnBehalfOrderModification },
            { new Tuple<ActivityTypeContract, OnBehalf>(ActivityTypeContract.OrderModificationPrice, OnBehalf.Yes), NotificationType.OnBehalfOrderModification },
            { new Tuple<ActivityTypeContract, OnBehalf>(ActivityTypeContract.OrderModificationRelatedOrderRemoved, OnBehalf.Yes), NotificationType.OnBehalfOrderModification },
            { new Tuple<ActivityTypeContract, OnBehalf>(ActivityTypeContract.OrderModificationValidity, OnBehalf.Yes), NotificationType.OnBehalfOrderModification },
            { new Tuple<ActivityTypeContract, OnBehalf>(ActivityTypeContract.OrderModificationVolume, OnBehalf.Yes), NotificationType.OnBehalfOrderModification },
            { new Tuple<ActivityTypeContract, OnBehalf>(ActivityTypeContract.OrderCancellation, OnBehalf.Yes), NotificationType.OnBehalfOrderCancellation },
            { new Tuple<ActivityTypeContract, OnBehalf>(ActivityTypeContract.OrderCancellationBecauseAccountIsNotValid, OnBehalf.Yes), NotificationType.OnBehalfOrderCancellation },
            { new Tuple<ActivityTypeContract, OnBehalf>(ActivityTypeContract.OrderCancellationBecauseBaseOrderCancelled, OnBehalf.Yes), NotificationType.OnBehalfOrderCancellation },
            { new Tuple<ActivityTypeContract, OnBehalf>(ActivityTypeContract.OrderCancellationBecauseConnectedOrderExecuted, OnBehalf.Yes), NotificationType.OnBehalfOrderCancellation },
            { new Tuple<ActivityTypeContract, OnBehalf>(ActivityTypeContract.OrderCancellationBecauseCorporateAction, OnBehalf.Yes), NotificationType.OnBehalfOrderCancellation },
            { new Tuple<ActivityTypeContract, OnBehalf>(ActivityTypeContract.OrderCancellationBecauseInstrumentInNotValid, OnBehalf.Yes), NotificationType.OnBehalfOrderCancellation },
            { new Tuple<ActivityTypeContract, OnBehalf>(ActivityTypeContract.OrderCancellationBecausePositionClosed, OnBehalf.Yes), NotificationType.OnBehalfOrderCancellation },
            { new Tuple<ActivityTypeContract, OnBehalf>(ActivityTypeContract.PositionClosing, OnBehalf.Yes), NotificationType.OnBehalfPositionClosing },
            { new Tuple<ActivityTypeContract, OnBehalf>(ActivityTypeContract.PositionPartialClosing, OnBehalf.Yes), NotificationType.OnBehalfPositionClosing },
            { new Tuple<ActivityTypeContract, OnBehalf>(ActivityTypeContract.PriceAlertTriggered, OnBehalf.No), NotificationType.PriceAlertTriggered },
        };

        // Activity description enrichments based on Activity type
        internal static readonly IReadOnlyDictionary<ActivityTypeContract, Func<ActivityEvent, string[]>> DescriptionEnrichments =
            new Dictionary<ActivityTypeContract, Func<ActivityEvent, string[]>>
        {
        };
    }
}
