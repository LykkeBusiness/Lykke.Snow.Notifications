using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Lykke.Snow.Mdm.Contracts.Api;
using Lykke.Snow.Notifications.Domain.Exceptions;
using Lykke.Snow.Notifications.Domain.Model;
using Lykke.Snow.Notifications.DomainServices.Services;
using Lykke.Snow.Notifications.Tests.Fakes;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Internal;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using Xunit;

namespace Lykke.Snow.Notifications.Tests
{
    public class MdmLocalizationDataProviderTests
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

        class LocalizationTestData : IEnumerable<object[]>
        {
            public IEnumerator<object[]> GetEnumerator()
            {
                yield return new object[] { JsonConvert.DeserializeObject<LocalizationData>(LocalizationJsonText)! };
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        [Fact]
        public void Initialization_ShouldThrowException_WhenPlatformKeyIsNotProvided()
        {
            Assert.Throws<ArgumentNullException>(() => CreateSut(localizationPlatformKeyArg: ""));
            Assert.Throws<ArgumentNullException>(() => CreateSut(localizationPlatformKeyArg: null));
        }
        
        #region Load()
        [Fact]
        public async Task Load_ShouldLoadLocalizationFileFromApi_IfLocalizationDataIsNotInitialized()
        {
            var stub = new LocalizationFilesBinaryApiStub(LocalizationJsonText);
            
            // create the class - the localization data will be null initially
            var sut = CreateSut(stub);
            
            await sut.Load();
            
            // assert that the api has been called
            Assert.Equal(1, stub.NumOfCalls);
        }
        #endregion
        

        #region LoadFromMdm()
        [Fact]
        public async Task LoadFromMdm_HappyPath_ShouldReturnParsedLocalizationData()
        {
            var stub = new LocalizationFilesBinaryApiStub(LocalizationJsonText);
            
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
            var stub = new LocalizationFilesBinaryApiStub(LocalizationJsonText);
            stub.SetJsonResponseText("");
            
            var sut = CreateSut(localizationFilesBinaryApiArg: stub);
            
            await Assert.ThrowsAsync<LocalizationFileParsingException>(() => sut.LoadFromMdm());
        }

        [Fact]
        public async Task LoadFromMdm_ShouldThrowLocalizationFileParsingException_IfJsonParsingFails()
        {
            // setup the stub so it returns an invalid json (not parsable into LocalizationData)
            var stub = new LocalizationFilesBinaryApiStub(LocalizationJsonText);
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
            var memoryCache = new MemoryCache(new MemoryCacheOptions());
            
            if(localizationFilesBinaryApiArg != null)
            {
                localizationFilesBinaryApi = localizationFilesBinaryApiArg;
            }
            
            if(cacheExpirationPeriodArg != null)
            {
                cacheExpirationPeriod = cacheExpirationPeriodArg.Value;
            }
            
            return new MdmLocalizationDataProvider(mockLogger.Object, 
                localizationFilesBinaryApi, 
                cacheExpirationPeriodArg, 
                localizationPlatformKey: localizationPlatformKeyArg,
                cache: memoryCache);
        }
    }
}
