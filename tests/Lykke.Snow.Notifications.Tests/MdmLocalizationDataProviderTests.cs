using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using Lykke.Snow.Mdm.Contracts.Api;
using Lykke.Snow.Notifications.Domain.Exceptions;
using Lykke.Snow.Notifications.Domain.Model;
using Lykke.Snow.Notifications.DomainServices.Services;
using Lykke.Snow.Notifications.Tests.Fakes;
using Microsoft.Extensions.Internal;
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
            const string LocalizationJsonText = @"
                {
                    ""Titles"": {
                        ""AccountLocked"": {
                           ""en"": ""Account locked"", 
                           ""es"": ""Coenta bloqueada"", 
                           ""de"": ""Konto gesperrt"", 
                        },
                        ""DepositSucceeded"": {
                           ""en"": ""Deposit Succeeded"", 
                           ""es"": ""Depósito exitoso"", 
                           ""de"": ""Einzahlung erfolgreich"", 
                        }
                    },
                    ""Bodies"": {
                       ""AccountLocked"": {
                           ""en"": ""Account has been locked."",
                           ""es"": ""La cuenta ha sido bloqueada"",
                           ""de"": ""Konto wurde gesperrt""
                       }, 
                       ""DepositSucceeded"": {
                           ""en"": ""{0}{1} has been deposited to the account {2}."",
                           ""es"": ""{0}{1} se ha depositado en la cuenta {2}."",
                           ""de"": ""{0}{1} wurde auf das Konto {2} eingezahlt.""
                       } 
                    }
                }
            ";

            yield return new object[] { JsonConvert.DeserializeObject<LocalizationData>(LocalizationJsonText)! };
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    public class MdmLocalizationDataProviderTests
    {
        [Fact]
        public void Initialization_ShouldThrowException_WhenPlatformKeyIsNotProvided()
        {
            Assert.Throws<ArgumentNullException>(() => CreateSut(localizationPlatformKeyArg: ""));
            Assert.Throws<ArgumentNullException>(() => CreateSut(localizationPlatformKeyArg: null));
        }

        [Fact]
        public void CacheExpirationTime_ShouldSetToDefault_WhenNotProvided()
        {
            var sut = CreateSut();
            
            var cacheExpirationPeriod = sut
                .GetType()
                .GetField("_cacheExpirationPeriod", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.GetValue(sut);    
            
            var defaultCacheExpirationPeriod = sut
                .GetType()
                .GetField("DefaultCacheExpirationPeriod", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.GetValue(sut);
            
            if(cacheExpirationPeriod == null)
                throw new NullReferenceException($"{nameof(cacheExpirationPeriod)} is null");

            if(defaultCacheExpirationPeriod == null)
                throw new NullReferenceException($"{nameof(defaultCacheExpirationPeriod)} is null");
            
            Assert.Equal((TimeSpan) cacheExpirationPeriod, (TimeSpan) defaultCacheExpirationPeriod);
        }
        
        #region Load()
        [Fact]
        public async Task Load_ShouldLoadLocalizationFileFromApi_IfLocalizationDataIsNotInitialized()
        {
            var stub = new LocalizationFilesBinaryApiStub();
            
            // create the class - the localization data will be null initially
            var sut = CreateSut(stub);
            
            await sut.Load();
            
            // assert that the api has been called
            Assert.Equal(1, stub.NumOfCalls);
        }
        
        [Theory]
        [ClassData(typeof(LocalizationTestData))]
        public async Task Load_ShouldReturnLocalizationDataFromMemory_IfDataHasNotExpired(LocalizationData localizationData)
        {
            // set expiration to 1 minute
            var expiration = new TimeSpan(0, 1, 0);
            
            var mockApi = new Mock<ILocalizationFilesBinaryApi>();
            
            var sut = CreateSut(cacheExpirationPeriodArg: expiration, localizationFilesBinaryApiArg: mockApi.Object);

            // Set value for 'localizationData' private field so that it is not null
            var dataField = typeof(MdmLocalizationDataProvider).GetField("_localizationData", BindingFlags.NonPublic | BindingFlags.Instance);
            if(dataField == null)
                throw new NullReferenceException($"{nameof(dataField)} is null");
            dataField.SetValue(sut, localizationData);
            
            //Set 'lastUpdated' private field to simulate a fresh (30 seconds old) localizationData
            var lastUpdatedField = typeof(MdmLocalizationDataProvider).GetField("_lastUpdatedAt", BindingFlags.NonPublic | BindingFlags.Instance);
            if(lastUpdatedField == null)
                throw new NullReferenceException($"{nameof(lastUpdatedField)} is null");
            lastUpdatedField.SetValue(sut, DateTime.UtcNow.AddSeconds(-30));
            
            var result = await sut.Load();
            
            // Verify that api has not been called
            mockApi.Verify(x => x.GetActiveLocalizationFileAsync(It.IsAny<string>()), Times.Never);
            
            Assert.Equal(localizationData, result);
        }
        
        [Theory]
        [ClassData(typeof(LocalizationTestData))]
        public async Task Load_ShouldLoadLocalizationFileFromApi_IfDataHasExpired(LocalizationData localizationData)
        {
            // set expiration to 1 minute
            var expiration = new TimeSpan(0, 1, 0);

            var stub = new LocalizationFilesBinaryApiStub();

            var sut = CreateSut(cacheExpirationPeriodArg: expiration, localizationFilesBinaryApiArg: stub);

            // Set value for 'localizationData' private field so that it is not null
            var dataField = typeof(MdmLocalizationDataProvider).GetField("_localizationData", BindingFlags.NonPublic | BindingFlags.Instance);
            if(dataField == null)
                throw new NullReferenceException($"{nameof(dataField)} is null");
            dataField.SetValue(sut, localizationData);

            //Set 'lastUpdated' private field to simulate a stale (90 seconds old) localizationData
            var lastUpdatedField = typeof(MdmLocalizationDataProvider).GetField("_lastUpdatedAt", BindingFlags.NonPublic | BindingFlags.Instance);
            if(lastUpdatedField == null)
                throw new NullReferenceException($"{nameof(lastUpdatedField)} is null");
            lastUpdatedField.SetValue(sut, DateTime.UtcNow.AddSeconds(-90));
            
            await sut.Load();
            
            Assert.Equal(1, stub.NumOfCalls);
        }
        

        [Fact]
        public async Task Load_ShouldUpdateLastUpdatedAt_AfterUpdatingTheCache()
        {
            var stub = new LocalizationFilesBinaryApiStub();
            
            var simulatedNow = new DateTime(2023, 05, 18, 14, 51, 22);

            var mockSystemClock = new Mock<ISystemClock>();
            mockSystemClock.Setup(x => x.UtcNow).Returns((DateTimeOffset)simulatedNow);

            // Create the sut without (the localizationData will be null initially - so that it will load the data from API)
            var sut = CreateSut(localizationFilesBinaryApiArg: stub, systemClockArg: mockSystemClock.Object);
            
            await sut.Load();

            // read lastUpdatedAt value
            var lastUpdatedAt = sut
                .GetType()
                .GetField("_lastUpdatedAt", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.GetValue(sut);
            
            if (lastUpdatedAt == null)
                throw new NullReferenceException($"{nameof(lastUpdatedAt)} is null");
            
            Assert.Equal(simulatedNow, (DateTime) lastUpdatedAt);
        }
        #endregion
        

        #region LoadFromMdm()
        [Fact]
        public async Task LoadFromMdm_HappyPath_ShouldReturnParsedLocalizationData()
        {
            var stub = new LocalizationFilesBinaryApiStub();
            
            var sut = CreateSut(localizationFilesBinaryApiArg: stub);
            
            var result = await sut.LoadFromMdm();
            
            Assert.NotNull(result);
            Assert.NotEmpty(result.Titles);
            Assert.NotEmpty(result.Bodies);
        }
        
        [Fact]
        public void LoadFromMdm_ShouldThrowLocalizationFileNotFoundException_IfHttpRequestFails()
        {
            var mockLocaliztaionFileBinaryApi = new Mock<ILocalizationFilesBinaryApi>();
            mockLocaliztaionFileBinaryApi.Setup(x => x.GetActiveLocalizationFileAsync(It.IsAny<string>()))
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.NotFound));
            
            var sut = CreateSut(localizationFilesBinaryApiArg: mockLocaliztaionFileBinaryApi.Object);
            
            Assert.ThrowsAsync<LocalizationFileCannotBeLoadedException>(() => sut.LoadFromMdm());
        }

        [Fact]
        public async Task LoadFromMdm_ShouldThrowLocalizationFileParsingException_IfResultIsNull()
        {
            // setup the stub so it simulates the api returning an empty string
            var stub = new LocalizationFilesBinaryApiStub();
            stub.SetJsonResponseText("");
            
            var sut = CreateSut(localizationFilesBinaryApiArg: stub);
            
            await Assert.ThrowsAsync<LocalizationFileParsingException>(() => sut.LoadFromMdm());
        }

        [Fact]
        public async Task LoadFromMdm_ShouldThrowLocalizationFileParsingException_IfJsonParsingFails()
        {
            // setup the stub so it returns an invalid json (not parsable into LocalizationData)
            var stub = new LocalizationFilesBinaryApiStub();
            stub.SetJsonResponseText(@"{""FirstName"": ""Jane"", ""LastName"": ""Doe""}");
            
            var sut = CreateSut(localizationFilesBinaryApiArg: stub);
            
            await Assert.ThrowsAsync<LocalizationFileParsingException>(() => sut.LoadFromMdm());
        }

        #endregion

        private MdmLocalizationDataProvider CreateSut(ILocalizationFilesBinaryApi? localizationFilesBinaryApiArg = null,
            TimeSpan? cacheExpirationPeriodArg = null,
            ISystemClock systemClockArg = null,
            string localizationPlatformKeyArg = "NotificationService")
        {
            var mockLogger = new Mock<ILogger<MdmLocalizationDataProvider>>();
            
            ILocalizationFilesBinaryApi localizationFilesBinaryApi = new Mock<ILocalizationFilesBinaryApi>().Object;
            TimeSpan cacheExpirationPeriod = TimeSpan.FromMinutes(5);
            ISystemClock systemClock = new SystemClock();
            
            if(localizationFilesBinaryApiArg != null)
            {
                localizationFilesBinaryApi = localizationFilesBinaryApiArg;
            }
            
            if(cacheExpirationPeriodArg != null)
            {
                cacheExpirationPeriod = cacheExpirationPeriodArg.Value;
            }
            
            if(systemClockArg != null)
            {
                systemClock = systemClockArg;
            }
            
            return new MdmLocalizationDataProvider(mockLogger.Object, localizationFilesBinaryApi, cacheExpirationPeriodArg, systemClock,
                localizationPlatformKey: localizationPlatformKeyArg);
        }
    }
}
