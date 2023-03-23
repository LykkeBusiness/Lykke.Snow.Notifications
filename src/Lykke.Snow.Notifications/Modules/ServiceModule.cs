// Copyright (c) 2020 Lykke Corp.
// See the LICENSE file in the project root for more information.

using Autofac;
using Lykke.Middlewares.Mappers;
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

            builder.RegisterType<LocalizationService>()
                .As<ILocalizationService>()
                .WithParameter("localizationFilePath", "localization.json")
                .SingleInstance();
        }
    }
}
