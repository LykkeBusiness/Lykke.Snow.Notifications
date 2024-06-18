using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
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
                ""Attributes"": {
                    ""BUY"": {
                       ""en"": ""BUY"", 
                       ""es"": ""COMPRAR"", 
                       ""de"": ""KAUF"", 
                    },
                    ""SELL"": {
                       ""en"": ""SELL"", 
                       ""es"": ""VENDER"", 
                       ""de"": ""VERKAUF"", 
                    },
                    ""LONG"": {
                       ""en"": ""LONG"", 
                       ""es"": ""LARGO"", 
                       ""de"": ""LANGE"", 
                    },
                    ""SHORT"": {
                       ""en"": ""SHORT"", 
                       ""es"": ""CORTO"", 
                       ""de"": ""KURZ"", 
                    }
                },
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

        class TranslateParametersTestData : IEnumerable<object[]>
        {
            public IEnumerator<object[]> GetEnumerator()
            {
                string[] translateAttributes = new string[] { "BUY", "SELL", "LONG", "SHORT" };
                var translateAttributesHashset = new HashSet<string>(translateAttributes, StringComparer.OrdinalIgnoreCase);

                yield return new object[]
                {
                    translateAttributesHashset, new [] { "BUY", "10", "PRODUCT A" }, "es", new [] { "COMPRAR", "10", "PRODUCT A" },
                };
                
                yield return new object[]
                {
                    translateAttributesHashset, new [] { "SELL", "5", "SHORT", "PRODUCT B" }, "de", new [] { "VERKAUF", "5", "KURZ", "PRODUCT B" }
                };

                yield return new object[]
                {
                    translateAttributesHashset, new [] { "Buy", "15", "PRODUCT C" }, "de", new [] { "KAUF", "15", "PRODUCT C" }
                };
                
                yield return new object[]
                {
                    translateAttributesHashset, new [] { "5", "lOnG", "PRODUCT D" }, "es", new [] { "5", "LARGO", "PRODUCT D" }
                };
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
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
        
        [Theory]
        [ClassData(typeof(TranslateParametersTestData))]
        public void TranslateParametersIfApplicable_ShouldTranslateParameters_GivenInTheTranslateAttributesCollection(
            HashSet<string> translateAttributes,
            string[] inputParameters,
            string lang,
            string[] expectedParameters
        )
        {
            var localizationData = JsonConvert.DeserializeObject<LocalizationData>(LocalizationJsonText)!;
            
            var sut = CreateSut(data: localizationData);
            
            var result = sut.TranslateParametersIfApplicable(localizationData, translateAttributes, inputParameters, lang);

            Assert.Equal(expectedParameters, result);
        }
        
        [Fact]
        public void TranslateAttributesArgument_ShouldBeTransformedToUpperCase_DuringInitialization()
        {
            var translateAttributes = new[] { "Buy", "selL", "lOng", "sHort" };

            var localizationData = JsonConvert.DeserializeObject<LocalizationData>(LocalizationJsonText)!;
            
            var sut = CreateSut(translateAttributesArg: translateAttributes, data: localizationData);

            var translateAttributesField = sut
                .GetType()
                .GetField("_translateAttributes", BindingFlags.NonPublic | BindingFlags.Instance)
                ?.GetValue(sut);
            
            if(translateAttributesField == null)
                throw new NullReferenceException(nameof(translateAttributesField));

            var translateAttributesTypedField = (HashSet<string>) translateAttributesField;
            
            Assert.Equal(translateAttributes.Length, translateAttributesTypedField.Count);
            
            foreach(var a in translateAttributes)
                Assert.Contains(a.ToUpper(), translateAttributesTypedField);
        }
        
        private LocalizationService CreateSut(LocalizationData data, string[] translateAttributesArg = null)
        {
            string[] translateAttributes = new string[]{};
            
            if(translateAttributesArg != null)
            {
                translateAttributes = translateAttributesArg;
            }

            var mockLogger = new Mock<ILogger<LocalizationService>>();

            var mockProvider = new Mock<ILocalizationDataProvider>();
            mockProvider.Setup(x => x.Load()).Returns(Task.FromResult(data));

            return new LocalizationService(mockLogger.Object, mockProvider.Object, translateAttributes);
        }
    }
}
