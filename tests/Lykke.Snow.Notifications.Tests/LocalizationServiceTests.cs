using System;
using System.Collections;
using System.Collections.Generic;
using Lykke.Snow.Notifications.DomainServices.Model;
using Lykke.Snow.Notifications.DomainServices.Services;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using Xunit;

namespace Lykke.Snow.Notifications.Tests
{
    class LocalizationTestData : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            var localizationJsonText = @"
                {
                    ""Titles"": {
                        ""AccountLocked"": {
                           ""en"": ""en- Account locked"", 
                           ""es"": ""es- Account locked"", 
                           ""de"": ""de- Account locked"", 
                        },
                        ""DepositSucceeded"": {
                           ""en"": ""en- Deposit Succ"", 
                           ""es"": ""es- Account locked"", 
                           ""de"": ""de- Account locked"", 
                        }
                    },
                    ""Bodies"": {
                       ""AccountLocked"": {
                           ""en"": ""en- Account has been locked."",
                           ""es"": ""es- Account has been locked."",
                           ""de"": ""de- Account has been locked.""
                       } 
                    }
                }
            ";
            
            yield return new object[] { JsonConvert.DeserializeObject<LocalizationData>(localizationJsonText) };
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    public class LocalizationServiceTests
    {
        
        [Theory]
        [ClassData(typeof(LocalizationTestData))]
        public void Test1(LocalizationData data)
        {
            //TODO complete unit tests
        }
        
        private LocalizationService CreateSut(LocalizationData localizationDataArg)
        {
            var mockLogger = new Mock<ILogger<LocalizationService>>();

            return new LocalizationService(localizationDataArg, mockLogger.Object);
        }
    }
}
