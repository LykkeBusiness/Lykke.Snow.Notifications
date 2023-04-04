using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.MarginTrading.Activities.Contracts.Models;
using Lykke.Snow.Notifications.Domain.Services;
using Lykke.Snow.Notifications.DomainServices.Projections;
using Moq;
using Xunit;

namespace Lykke.Snow.Notifications.Tests
{
    class ActivityEventTestData : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            yield return new object[] 
            { 
                new ActivityEvent
                {
                    Id = "some-id",
                    Timestamp = DateTime.UtcNow,
                    Activity = new ActivityContract("id", "account-id", "instrument", "event-source-id", DateTime.UtcNow, ActivityCategoryContract.CashMovement,
                        ActivityTypeContract.OrderExpiry, new[] { "attr-1", "attr-2"}, new[] { "relatedid-1", "relatedid-2" })
                }
            };
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    public class ActivityProjectionTests
    {
        [Theory]
        [ClassData(typeof(ActivityEventTestData))]
        public async Task Handle_ShouldPassActivityEvent_ToActivityHandler(ActivityEvent e)
        {
            var activityHandlerMock = new Mock<IActivityHandler>();
            
            var sut = CreateSut(activityHandlerMock.Object);
            
            await sut.Handle(e);
            
            activityHandlerMock.Verify(x => x.Handle(e), Times.Once);
        }
        
        private ActivityProjection CreateSut(IActivityHandler activityHandlerArg = null)
        {
            IActivityHandler activityHandler = new Mock<IActivityHandler>().Object;
            
            if(activityHandlerArg != null)
            {
                activityHandler = activityHandlerArg;
            }
            
            return new ActivityProjection(activityHandler);
        }
    }
}
