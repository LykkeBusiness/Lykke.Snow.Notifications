// Copyright (c) 2020 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using Lykke.MarginTrading.Activities.Contracts.Models;
using Lykke.Middlewares.Mappers;
using Lykke.Snow.Notifications.Domain.Enums;
using Lykke.Snow.Notifications.Domain.Services;
using Lykke.Snow.Notifications.DomainServices.Services;
using Microsoft.Extensions.Internal;

namespace Lykke.Snow.Notifications.Modules
{
    internal class ServiceModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<DefaultHttpStatusCodeMapper>()
                .As<IHttpStatusCodeMapper>()
                .SingleInstance();

            builder.RegisterType<DefaultLogLevelMapper>()
                .As<ILogLevelMapper>()
                .SingleInstance();

            builder.RegisterType<SystemClock>()
                .As<ISystemClock>()
                .SingleInstance();
            
            builder.RegisterType<StartupManager>()
                .AsSelf()
                .SingleInstance();

            builder.RegisterType<DeviceRegistrationService>()
                .As<IDeviceRegistrationService>()
                .SingleInstance();

            builder.RegisterType<LocalizationService>()
                .As<ILocalizationService>()
                .SingleInstance();

            builder.RegisterType<FileSystemLocalizationDataProvider>()
                .As<ILocalizationDataProvider>()
                .WithParameter("localizationFilePath", "localization.json")
                .SingleInstance();

            builder.RegisterType<ActivityHandler>()
                .As<IActivityHandler>()
                .WithParameter("notificationTypeMapping", new Dictionary<ActivityTypeContract, NotificationType>
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
                    { ActivityTypeContract.PositionPartialClosing, NotificationType.PositionClosed },
                    { ActivityTypeContract.PriceAlertTriggered, NotificationType.PriceAlertTriggered }
                } as IReadOnlyDictionary<ActivityTypeContract, NotificationType>)
                .WithParameter("descriptionEnrichments", new Dictionary<ActivityTypeContract, Func<ActivityEvent, string[]>>
                {
                    {ActivityTypeContract.AccountWithdrawalSucceeded, (e) => { return e.Activity.DescriptionAttributes.ToList().Append(e.Activity.AccountId).ToArray(); }},
                    {ActivityTypeContract.AccountDepositSucceeded, (e) => { return e.Activity.DescriptionAttributes.ToList().Append(e.Activity.AccountId).ToArray(); }},
                    {ActivityTypeContract.AccountWithdrawalEnabled, (e) => { return e.Activity.DescriptionAttributes.ToList().Append(e.Activity.AccountId).ToArray(); }},
                    {ActivityTypeContract.AccountWithdrawalDisabled, (e) => { return e.Activity.DescriptionAttributes.ToList().Append(e.Activity.AccountId).ToArray(); }},
                    {ActivityTypeContract.AccountTradingEnabled, (e) => { return e.Activity.DescriptionAttributes.ToList().Append(e.Activity.AccountId).ToArray(); }},
                    {ActivityTypeContract.AccountTradingDisabled, (e) => { return e.Activity.DescriptionAttributes.ToList().Append(e.Activity.AccountId).ToArray(); }}
                } as IReadOnlyDictionary<ActivityTypeContract, Func<ActivityEvent, string[]>>)
                .SingleInstance();

            builder.RegisterType<MessagePreviewEventHandler>()
                .As<IMessagePreviewEventHandler>()
                .SingleInstance();
        }
    }
}
