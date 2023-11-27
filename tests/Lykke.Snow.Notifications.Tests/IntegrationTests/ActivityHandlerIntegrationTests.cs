using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using Lykke.MarginTrading.Activities.Contracts.Models;
using Lykke.Snow.FirebaseIntegration.Interfaces;
using Lykke.Snow.Notifications.Domain.Services;
using Lykke.Snow.Notifications.Tests.Fakes;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Lykke.Snow.Notifications.Tests.IntegrationTests
{
    public class ActivityHandlerIntegrationTests : IntegrationTestsBase
    {
        private readonly IActivityHandler _activityHandler;
        private readonly FcmIntegrationServiceFake _fcmIntegrationService;
        private readonly string _localizations;

        public ActivityHandlerIntegrationTests(WebAppFactoryNoDependencies factory)
        {
            _localizations = File.ReadAllText("localization.json");
            _activityHandler = factory.Services.GetRequiredService<IActivityHandler>();
            _fcmIntegrationService = (FcmIntegrationServiceFake)factory.Services.GetRequiredService<IFcmIntegrationService>();
        }

        [Theory, AutoData]
        public async Task ActivityHandler_Should_Send_Correct_OnBehalf_Notification(ActivityEvent activityEvent)
        {
            AssetApi.GetLegacyAssetByIdEndpoint.RespondWith(new { contractSize = 1 });
            LocalizationFilesApi.GetActiveLocalizationFileEndpoint.RespondWith(_localizations);

            var createdActivity = activityEvent.Activity;
            var descriptionAttributes = Enumerable.Range(1, 4).Select(i => Guid.NewGuid().ToString()).ToArray();
            activityEvent.Activity = new ActivityContract(createdActivity.Id, "account-id-1",
                createdActivity.Instrument, createdActivity.EventSourceId, createdActivity.Timestamp,
                ActivityCategoryContract.Order, ActivityTypeContract.OrderAcceptanceAndExecution,
                descriptionAttributes, createdActivity.RelatedIds, true);

            await _activityHandler.Handle(activityEvent);

            const string expectedTitle = "On Behalf Order Execution";
            var expectedBody = string.Format("{0} {1} order for {2} {3} has been executed.", descriptionAttributes);
            var atLeastOneSent = _fcmIntegrationService.ReceivedMessages.Any(message => message.Notification.Title == expectedTitle && message.Notification.Body == expectedBody);
            Assert.True(atLeastOneSent);
        }
    }
}
