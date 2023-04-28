using System;
using System.Collections.Generic;
using System.Linq;
using Lykke.MarginTrading.Activities.Contracts.Models;
using Lykke.Snow.Notifications.Domain.Enums;

namespace Lykke.Snow.Notifications.DomainServices.Mapping
{
    internal static class ActivityTypeMapping
    {
        // we may need to add more keys here
        // for example
        // positionClosing - not on behalf - NotificationType.PositionClosed
        // positionClosing - on behalf - NotificationType.OnBehalfPositionClosing
        // TODO: rename IsOnBehalf -> OnBehalf.No / OnBehalf.Yes
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
            { new Tuple<ActivityTypeContract, OnBehalf>(ActivityTypeContract.PositionPartialClosing, OnBehalf.No), NotificationType.PositionClosed }
        };

        // Activity description enrichments based on Activity type
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
        
        // Activity description interceptors based on Notification type
        // TODO: we might not need this
        //internal static readonly IReadOnlyDictionary<NotificationType, Func<ActivityEvent, string[]>> DescriptionInterceptors = 
        //    new Dictionary<NotificationType, Func<ActivityEvent, string[]>>
        //{
        //};
    }
}
