using Autofac;
using Lykke.Common.Api.Contract.Responses;
using Lykke.HttpClientGenerator;
using Lykke.Snow.Mdm.Contracts.Api;
using Lykke.Snow.Notifications.Settings;

namespace Lykke.Snow.Notifications.Modules
{
    internal class ExternalServicesModule : Module
    {
        private readonly NotificationServiceSettings _settings;

        public ExternalServicesModule(NotificationServiceSettings settings)
        {
            _settings = settings;
        }

        protected override void Load(ContainerBuilder builder)
        {
            var mdmClientGenerator = HttpClientGenerator.HttpClientGenerator
                .BuildForUrl(_settings.MdmServiceClient.ServiceUrl)
                .WithServiceName<ErrorResponse>("Mdm Service")
                .WithOptionalApiKey(_settings.MdmServiceClient.ApiKey)
                .Create();
            
            
            builder.RegisterInstance(mdmClientGenerator.Generate<ILocalizationFilesBinaryApi>());
        }
    }
}
