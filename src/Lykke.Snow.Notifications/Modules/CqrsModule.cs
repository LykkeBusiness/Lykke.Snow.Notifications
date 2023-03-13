using System;
using Autofac;
using Lykke.Cqrs;
using Lykke.Cqrs.Configuration;
using Lykke.Cqrs.Configuration.Routing;
using Lykke.Cqrs.Middleware.Logging;
using Lykke.Messaging.Serialization;
using Lykke.Snow.Common.Correlation;
using Lykke.Snow.Common.Correlation.Cqrs;
using Lykke.Snow.Common.Correlation.Http;
using Lykke.Snow.Common.Correlation.RabbitMq;
using Lykke.Snow.Cqrs;
using Lykke.Snow.Notifications.Settings;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace Lykke.Snow.Notifications.Modules
{
    public class CqrsModule : Module
    {
        private const string DefaultRoute = "self";
        private const string DefaultPipeline = "commands";
        private const string DefaultEventPipeline = "events";
        private readonly CqrsContextNamesSettings _contextNames;
        private readonly long _defaultRetryDelayMs;
        private readonly CqrsSettings _settings;

        public CqrsModule(CqrsSettings settings)
        {
            _settings = settings;
            _contextNames = settings.ContextNames;
            _defaultRetryDelayMs = (long) _settings.RetryDelay.TotalMilliseconds;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(context => new AutofacDependencyResolver(context)).As<IDependencyResolver>()
               .SingleInstance();

            builder.RegisterType<CqrsCorrelationManager>()
               .AsSelf()
               .SingleInstance();

            builder.RegisterType<CorrelationContextAccessor>()
               .AsSelf()
               .SingleInstance();

            builder.RegisterType<RabbitMqCorrelationManager>()
               .AsSelf()
               .SingleInstance();

            builder.RegisterType<HttpCorrelationHandler>()
               .AsSelf()
               .InstancePerDependency();

            builder.Register(CreateEngine)
               .As<ICqrsEngine>()
               .SingleInstance();
        }
        
        private CqrsEngine CreateEngine(IComponentContext ctx)
        {
            var rabbitMqConventionEndpointResolver = new RabbitMqConventionEndpointResolver(
                "RabbitMq",
                SerializationFormat.MessagePack,
                environment: _settings.EnvironmentName);
            
            var rabbitMqSettings = new ConnectionFactory
            {
                Uri = new Uri(_settings.ConnectionString, UriKind.Absolute)
            };
            
            var loggerFactory = ctx.Resolve<ILoggerFactory>();

            IRegistration[] registrations = new IRegistration[] 
            {
                RegisterDefaultRouting(),
                RegisterContext(),
                Register.CommandInterceptors(new DefaultCommandLoggingInterceptor(loggerFactory)),
                Register.EventInterceptors(new DefaultEventLoggingInterceptor(loggerFactory))
            };
            
            var engine = new RabbitMqCqrsEngine(loggerFactory,
                ctx.Resolve<IDependencyResolver>(),
                new DefaultEndpointProvider(),
                rabbitMqSettings.Endpoint.ToString(),
                rabbitMqSettings.UserName,
                rabbitMqSettings.Password,
                createMissingEndpoints: true,
                registrations
                );
            
            var correlationManager = ctx.Resolve<CqrsCorrelationManager>();
            engine.SetWriteHeadersFunc(correlationManager.BuildCorrelationHeadersIfExists);
            engine.SetReadHeadersAction(correlationManager.FetchCorrelationIfExists);

            return engine;
        }

        private PublishingCommandsDescriptor<IDefaultRoutingRegistration> RegisterDefaultRouting()
        {
            return Register.DefaultRouting
                .PublishingCommands(
                )
                .To(_contextNames.NotificationsService)
                .With(DefaultPipeline);
        }
        
        private IRegistration RegisterContext()
        {
            var contextRegistration = Register.BoundedContext(_contextNames.NotificationsService)
              .FailedCommandRetryDelay(_defaultRetryDelayMs)
              .ProcessingOptions(DefaultRoute).MultiThreaded(8).QueueCapacity(1024);

            return contextRegistration;
        }
    }
}
