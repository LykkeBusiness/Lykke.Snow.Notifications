using System;
using System.Collections;
using System.Collections.Generic;
using Lykke.Snow.Notifications.Domain.Exceptions;
using Lykke.Snow.Notifications.DomainServices.Model;
using Lykke.Snow.Notifications.DomainServices.Services;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using Xunit;

namespace Lykke.Snow.Notifications.Tests
{

    public class LocalizationServiceTests
    {
        const string localizationJsonText = @"
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
                yield return new object[] { JsonConvert.DeserializeObject<LocalizationData>(localizationJsonText) ?? throw new ArgumentNullException(), "AccountLocked", "en", new string[]{}, "Account locked", "Account has been locked." };
                yield return new object[] { JsonConvert.DeserializeObject<LocalizationData>(localizationJsonText) ?? throw new ArgumentNullException(), "AccountLocked", "es", new string[]{}, "Coenta bloqueada", "La cuenta ha sido bloqueada" };
                yield return new object[] { JsonConvert.DeserializeObject<LocalizationData>(localizationJsonText) ?? throw new ArgumentNullException(), "AccountLocked", "de", new string[]{}, "Konto gesperrt", "Konto wurde gesperrt" };
                yield return new object[] { JsonConvert.DeserializeObject<LocalizationData>(localizationJsonText) ?? throw new ArgumentNullException(), "DepositSucceeded", "en", new string[]{ "100", "EUR", "A001" }, "Deposit Succeeded", "100EUR has been deposited to the account A001." };
                yield return new object[] { JsonConvert.DeserializeObject<LocalizationData>(localizationJsonText) ?? throw new ArgumentNullException(), "DepositSucceeded", "es", new string[]{ "100", "EUR", "A001" }, "Depósito exitoso", "100EUR se ha depositado en la cuenta A001." };
                yield return new object[] { JsonConvert.DeserializeObject<LocalizationData>(localizationJsonText) ?? throw new ArgumentNullException(), "DepositSucceeded", "de", new string[]{ "100", "EUR", "A001" }, "Einzahlung erfolgreich", "100EUR wurde auf das Konto A001 eingezahlt." };
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        class InvalidNotificationTypeTestData : IEnumerable<object[]>
        {
            public IEnumerator<object[]> GetEnumerator()
            {
                yield return new object[] { JsonConvert.DeserializeObject<LocalizationData>(localizationJsonText) ?? throw new ArgumentNullException(), "en", "notification-type-that-does-not-exist" };
                yield return new object[] { JsonConvert.DeserializeObject<LocalizationData>(localizationJsonText) ?? throw new ArgumentNullException(), "es", "notification-type-that-does-not-exist" };
                yield return new object[] { JsonConvert.DeserializeObject<LocalizationData>(localizationJsonText) ?? throw new ArgumentNullException(), "de", "notification-type-that-does-not-exist" };
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        class InvalidLanguageTestData : IEnumerable<object[]>
        {
            public IEnumerator<object[]> GetEnumerator()
            {
                yield return new object[] { JsonConvert.DeserializeObject<LocalizationData>(localizationJsonText) ?? throw new ArgumentNullException(), "language-that-does-not-exist", "AccountLocked" };
                yield return new object[] { JsonConvert.DeserializeObject<LocalizationData>(localizationJsonText) ?? throw new ArgumentNullException(), "language-that-does-not-exist", "DepositSucceeded" };
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        class InvalidTranslationArgumentsTestData : IEnumerable<object[]>
        {
            public IEnumerator<object[]> GetEnumerator()
            {
                yield return new object[] { JsonConvert.DeserializeObject<LocalizationData>(localizationJsonText) ?? throw new ArgumentNullException(), "en", "DepositSucceeded", new string [] { "we pass two args", "while we should've three" } };
                yield return new object[] { JsonConvert.DeserializeObject<LocalizationData>(localizationJsonText) ?? throw new ArgumentNullException(), "es", "DepositSucceeded", new string [] { "we pass two args", "while we should've three" } };
                yield return new object[] { JsonConvert.DeserializeObject<LocalizationData>(localizationJsonText) ?? throw new ArgumentNullException(), "de", "DepositSucceeded", new string [] { "we pass two args", "while we should've three" } };
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }
        
        [Theory]
        [ClassData(typeof(LocalizationHappyPathTestData))]
        public void GetLocalizedText_HappyPath_ShouldReturnTranslatedMessage(LocalizationData data, string type, string lang, string[] args, string expectedTitle, string expectedBody)
        {
            var sut = CreateSut(data);
            
            var result = sut.GetLocalizedText(type, lang, args);
            
            Assert.Equal(expectedTitle, result.Item1);
            Assert.Equal(expectedBody, result.Item2);
        }
        
        [Theory]
        [ClassData(typeof(InvalidNotificationTypeTestData))]
        public void GetLocalizedText_ShouldThrowTranslationNotFoundException_WhenNotificationTypeIsNotPresent(LocalizationData data, string lang, string invalidNotificationType)
        {
            var sut = CreateSut(data);
            
            Assert.Throws<TranslationNotFoundException>(() => sut.GetLocalizedText(invalidNotificationType, lang, new string[] {}));
        }

        [Theory]
        [ClassData(typeof(InvalidLanguageTestData))]
        public void GetLocalizedText_ShouldThrowTranslationNotFoundException_WhenLanguageIsNotPresent(LocalizationData data, string invalidLanguage, string notificationType)
        {
            var sut = CreateSut(data);
            
            Assert.Throws<TranslationNotFoundException>(() => sut.GetLocalizedText(notificationType, invalidLanguage, new string[] {}));
        }

        [Theory]
        [ClassData(typeof(InvalidTranslationArgumentsTestData))]
        public void GetLocalizedText_ShouldThrowLocalizationFormatException_WhenNumberOfArgumentsDontMatch(LocalizationData data, string language, string notificationType, string[] args)
        {
            var sut = CreateSut(data);
            
            var ex = Assert.Throws<LocalizationFormatException>(() => sut.GetLocalizedText(notificationType, language, args));
        }
        
        private LocalizationService CreateSut(LocalizationData localizationDataArg)
        {
            var mockLogger = new Mock<ILogger<LocalizationService>>();

            return new LocalizationService(localizationDataArg, mockLogger.Object);
        }
    }
}
