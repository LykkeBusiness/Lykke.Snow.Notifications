using Autofac;
using Lykke.Common.Api.Contract.Responses;
using Lykke.HttpClientGenerator;
using Lykke.Snow.Mdm.Contracts.Api;
using Lykke.Snow.Notifications.Settings;
using MarginTrading.AssetService.Contracts;

namespace Lykke.Snow.Notifications.Modules
{
    internal class ExternalServicesModule(NotificationServiceSettings settings) : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            var mdmClientGenerator = HttpClientGenerator.HttpClientGenerator
                .BuildForUrl(settings.MdmServiceClient.ServiceUrl)
                .WithServiceName<ErrorResponse>("Mdm Service")
                .WithOptionalApiKey(settings.MdmServiceClient.ApiKey)
                .Create();
            
            builder.RegisterInstance(mdmClientGenerator.Generate<ILocalizationFilesBinaryApi>());
            
            var assetServiceClientGenerator = HttpClientGenerator.HttpClientGenerator
                .BuildForUrl(settings.AssetServiceClient.ServiceUrl)
                .WithServiceName<ErrorResponse>("MT Core Asset Service")
                .WithOptionalApiKey(settings.AssetServiceClient.ApiKey)
                .Create();
            
            builder.RegisterInstance(assetServiceClientGenerator.Generate<IAssetsApi>());
        }
    }
}
