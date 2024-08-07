// Copyright (c) 2020 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System;
using Autofac;
using Lykke.Middlewares.Mappers;
using Lykke.Snow.Notifications.Domain.Services;
using Lykke.Snow.Notifications.DomainServices.Services;
using Lykke.Snow.Notifications.Settings;
using Microsoft.Extensions.Internal;

namespace Lykke.Snow.Notifications.Modules
{
    internal class ServiceModule(NotificationServiceSettings notificationServiceSettings) : Module
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
                .WithParameter(new TypedParameter(typeof(string[]), notificationServiceSettings.Localization.TranslateAttributes))
                .SingleInstance();

            builder.RegisterType<MdmLocalizationDataProvider>()
                .As<ILocalizationDataProvider>()
                .WithParameter(new TypedParameter(typeof(TimeSpan?), notificationServiceSettings.Localization.LocalizationFileCache?.ExpirationPeriod))
                .WithParameter(new TypedParameter(typeof(string), notificationServiceSettings.Localization.LocalizationPlatformKey))
                .SingleInstance();

            builder.RegisterType<ActivityHandler>()
                .As<IActivityHandler>()
                .SingleInstance();
            
            builder.RegisterType<AssetService>()
                .As<IAssetService>()
                .SingleInstance();
        }
    }
}
