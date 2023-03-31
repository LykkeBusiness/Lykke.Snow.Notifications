using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Snow.Notifications.Domain.Exceptions;
using Lykke.Snow.Notifications.Domain.Model;
using Lykke.Snow.Notifications.Domain.Services;
using Lykke.Snow.Notifications.DomainServices.Services;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using Xunit;

namespace Lykke.Snow.Notifications.Tests
{

    public class LocalizationServiceTests
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
                
        class LocalizationHappyPathTestData : IEnumerable<object[]>
        {
            public IEnumerator<object[]> GetEnumerator()
            {
                yield return new object[]
                {
                    JsonConvert.DeserializeObject<LocalizationData>(LocalizationJsonText)!, "AccountLocked", "en",
                    new string[] { }, "Account locked", "Account has been locked."
                };
                yield return new object[]
                {
                    JsonConvert.DeserializeObject<LocalizationData>(LocalizationJsonText)!, "AccountLocked", "es",
                    new string[] { }, "Coenta bloqueada", "La cuenta ha sido bloqueada"
                };
                yield return new object[]
                {
                    JsonConvert.DeserializeObject<LocalizationData>(LocalizationJsonText)!, "AccountLocked", "de",
                    new string[] { }, "Konto gesperrt", "Konto wurde gesperrt"
                };
                yield return new object[]
                {
                    JsonConvert.DeserializeObject<LocalizationData>(LocalizationJsonText)!, "DepositSucceeded",
                    "en", new[] { "100", "EUR", "A001" }, "Deposit Succeeded",
                    "100EUR has been deposited to the account A001."
                };
                yield return new object[]
                {
                    JsonConvert.DeserializeObject<LocalizationData>(LocalizationJsonText)!, "DepositSucceeded",
                    "es", new[] { "100", "EUR", "A001" }, "Depósito exitoso",
                    "100EUR se ha depositado en la cuenta A001."
                };
                yield return new object[]
                {
                    JsonConvert.DeserializeObject<LocalizationData>(LocalizationJsonText)!, "DepositSucceeded",
                    "de", new[] { "100", "EUR", "A001" }, "Einzahlung erfolgreich",
                    "100EUR wurde auf das Konto A001 eingezahlt."
                };
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        class InvalidNotificationTypeTestData : IEnumerable<object[]>
        {
            public IEnumerator<object[]> GetEnumerator()
            {
                yield return new object[]
                {
                    JsonConvert.DeserializeObject<LocalizationData>(LocalizationJsonText)!, "en",
                    "notification-type-that-does-not-exist"
                };
                yield return new object[]
                {
                    JsonConvert.DeserializeObject<LocalizationData>(LocalizationJsonText)!, "es",
                    "notification-type-that-does-not-exist"
                };
                yield return new object[]
                {
                    JsonConvert.DeserializeObject<LocalizationData>(LocalizationJsonText)!, "de",
                    "notification-type-that-does-not-exist"
                };
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        class InvalidLanguageTestData : IEnumerable<object[]>
        {
            public IEnumerator<object[]> GetEnumerator()
            {
                yield return new object[]
                {
                    JsonConvert.DeserializeObject<LocalizationData>(LocalizationJsonText)!,
                    "language-that-does-not-exist", "AccountLocked"
                };
                yield return new object[]
                {
                    JsonConvert.DeserializeObject<LocalizationData>(LocalizationJsonText)!,
                    "language-that-does-not-exist", "DepositSucceeded"
                };
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        class InvalidTranslationArgumentsTestData : IEnumerable<object[]>
        {
            public IEnumerator<object[]> GetEnumerator()
            {
                yield return new object[]
                {
                    JsonConvert.DeserializeObject<LocalizationData>(LocalizationJsonText)!, "en",
                    "DepositSucceeded", new[] { "we pass two args", "while we should've three" }
                };
                yield return new object[]
                {
                    JsonConvert.DeserializeObject<LocalizationData>(LocalizationJsonText)!, "es",
                    "DepositSucceeded", new[] { "we pass two args", "while we should've three" }
                };
                yield return new object[]
                {
                    JsonConvert.DeserializeObject<LocalizationData>(LocalizationJsonText)!, "de",
                    "DepositSucceeded", new[] { "we pass two args", "while we should've three" }
                };
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }
        [Fact]
        public async Task GetLocalizedText_ShouldCallLoad_IfLocalizationDataIsNull()
        {
            var mockDataProvider = new Mock<ILocalizationDataProvider>();
            mockDataProvider.Setup(x => x.Load()).ReturnsAsync(new LocalizationData(
                titles: new Dictionary<string, IReadOnlyDictionary<string, string>>
                {
                    { "any-type", new Dictionary<string, string> { { "any-lang", "any-title" } } }
                },
                bodies: new Dictionary<string, IReadOnlyDictionary<string, string>>
                {
                    { "any-type", new Dictionary<string, string> { { "any-lang", "any-body" } } }
                }));

            var sut = CreateSut(mockDataProvider.Object);
            
            await sut.GetLocalizedTextAsync("any-type", "any-lang", new string[]{});
            await sut.GetLocalizedTextAsync("any-type", "any-lang", new string[]{});

            mockDataProvider.Verify(x => x.Load(), Times.Once);
        }

        [Theory]
        [ClassData(typeof(LocalizationHappyPathTestData))]
        public async Task GetLocalizedText_HappyPath_ShouldReturnTranslatedMessage(LocalizationData data,
            string type,
            string lang,
            string[] args,
            string expectedTitle,
            string expectedBody)
        {
            var sut = CreateSut(data);

            var result = await sut.GetLocalizedTextAsync(type, lang, args);

            Assert.Equal(expectedTitle, result.Item1);
            Assert.Equal(expectedBody, result.Item2);
        }

        [Theory]
        [ClassData(typeof(InvalidNotificationTypeTestData))]
        public void GetLocalizedText_ShouldThrowTranslationNotFoundException_WhenNotificationTypeIsNotPresent(
            LocalizationData data,
            string lang,
            string invalidNotificationType)
        {
            var sut = CreateSut(data);

            Assert.ThrowsAsync<TranslationNotFoundException>(() =>
                sut.GetLocalizedTextAsync(invalidNotificationType, lang, new string[] { }));
        }

        [Theory]
        [ClassData(typeof(InvalidLanguageTestData))]
        public void GetLocalizedText_ShouldThrowTranslationNotFoundException_WhenLanguageIsNotPresent(
            LocalizationData data,
            string invalidLanguage,
            string notificationType)
        {
            var sut = CreateSut(data);

            Assert.ThrowsAsync<TranslationNotFoundException>(() =>
                sut.GetLocalizedTextAsync(notificationType, invalidLanguage, new string[] { }));
        }

        [Theory]
        [ClassData(typeof(InvalidTranslationArgumentsTestData))]
        public void GetLocalizedText_ShouldThrowLocalizationFormatException_WhenNumberOfArgumentsDontMatch(
            LocalizationData data,
            string language,
            string notificationType,
            string[] args)
        {
            var sut = CreateSut(data);

            Assert.ThrowsAsync<LocalizationFormatException>(() =>
                sut.GetLocalizedTextAsync(notificationType, language, args));
        }

        private LocalizationService CreateSut(LocalizationData data)
        {
            var mockLogger = new Mock<ILogger<LocalizationService>>();

            var mockProvider = new Mock<ILocalizationDataProvider>();
            mockProvider.Setup(x => x.Load()).Returns(Task.FromResult(data));

            return new LocalizationService(mockLogger.Object, mockProvider.Object);
        }

        private LocalizationService CreateSut(ILocalizationDataProvider dataProvider)
        {
            var mockLogger = new Mock<ILogger<LocalizationService>>();

            return new LocalizationService(mockLogger.Object, dataProvider);
        }
    }
}
