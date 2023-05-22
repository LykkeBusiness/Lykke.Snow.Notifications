using System.Net.Http;
using System.Threading.Tasks;
using Lykke.Snow.Mdm.Contracts.Api;
using Lykke.Snow.Mdm.Contracts.Models.Contracts;
using Lykke.Snow.Mdm.Contracts.Models.Requests;
using Lykke.Snow.Mdm.Contracts.Models.Responses;
using Refit;

namespace Lykke.Snow.Notifications.Tests.Fakes
{
    public class LocalizationFilesBinaryApiStub : ILocalizationFilesBinaryApi
    {
        public int NumOfCalls { get; private set; } = 0;
        private string LocalizationJsonText = @"
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

        public Task<ErrorCodeResponse<LocalizationFileErrorCodesContract>> DeleteLocalizationFileAsync(string fileId, [Body] DeleteLocalizationFileRequest request)
        {
            throw new System.NotImplementedException();
        }

        public Task<ErrorCodeResponse<LocalizationFileErrorCodesContract>> DeleteLocalizationFilesBatchAsync([Body] DeleteLocalizationFilesBatchRequest request)
        {
            throw new System.NotImplementedException();
        }

        public Task<DiffLocalizationFileInfoResponse> DiffLocalizationFileAsync(string platform, StreamPart file)
        {
            throw new System.NotImplementedException();
        }

        public Task<HttpResponseMessage> GetActiveCompiledLocalizationFileAsync(string platform, [Query] GetActiveCompiledLocalizationFileRequest request)
        {
            throw new System.NotImplementedException();
        }

        public Task<HttpResponseMessage> GetActiveLocalizationFileAsync(string platform)
        {
            var response = new HttpResponseMessage(System.Net.HttpStatusCode.OK)
            {
                Content = new StringContent(LocalizationJsonText)
            };
            
            NumOfCalls++;

            return Task.FromResult(response);
        }

        public Task<HttpResponseMessage> GetLocalizationFileAsync(string fileId)
        {
            throw new System.NotImplementedException();
        }

        public Task<PaginatedResponseContract<LocalizationFileMetadataContract>> GetLocalizationFileInfosAsync([Query] GetLocalizationFileInfoRequest request)
        {
            throw new System.NotImplementedException();
        }

        public Task<ErrorCodeResponse<LocalizationFileErrorCodesContract>> UploadLocalizationFileAsync(string platform, StreamPart file, [Query] UploadLocalizationFileRequest request)
        {
            throw new System.NotImplementedException();
        }
        
        public void SetJsonResponseText(string json)
        {
            LocalizationJsonText = json;
        }
    }
}
