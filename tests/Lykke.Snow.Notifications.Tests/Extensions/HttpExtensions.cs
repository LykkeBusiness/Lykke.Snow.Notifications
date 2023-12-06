using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Lykke.Contracts.Responses;
using Newtonsoft.Json;
using Xunit;

namespace Lykke.Snow.Notifications.Tests.Extensions
{
    internal static class HttpExtensions
    {
        internal static async Task<T> ReadAsAsync<T>(this HttpContent content) where T : class
        {
            var json = await content.ReadAsStringAsync();
            if (string.IsNullOrEmpty(json))
                throw new InvalidOperationException("Content is empty");

            var result = JsonConvert.DeserializeObject<T>(json);
            if (result == null)
                throw new InvalidOperationException("Content is not a valid JSON");

            return result;
        }

        public static async Task AssertSuccessAsync<TError>(this HttpResponseMessage response, TError successValue)
        {
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var result = await response.Content.ReadAsAsync<ErrorCodeResponse<TError>>();
            Assert.Equal(successValue, result.ErrorCode);
        }

        public static async Task AssertErrorAsync<TError>(this HttpResponseMessage response, TError errorCode)
        {
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var result = await response.Content.ReadAsAsync<ErrorCodeResponse<TError>>();
            Assert.Equal(errorCode, result.ErrorCode);
        }

        public static async Task AssertAsync<T>(this HttpResponseMessage response, Action<T> assert) where T : class
        {
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var result = await response.Content.ReadAsAsync<T>();
            assert(result);
        }

        public static void AssertHttpStatusCode(this HttpResponseMessage response, HttpStatusCode statusCode)
        {
            Assert.Equal(statusCode, response.StatusCode);
        }
        
        public static Task<HttpResponseMessage> DeleteWithPayloadAsync<T>(this HttpClient client, string uri, T obj)
        {
            var request = new HttpRequestMessage
            {
                Content = new StringContent(JsonConvert.SerializeObject(obj), Encoding.UTF8, "application/json"),
                Method = HttpMethod.Delete,
                RequestUri = new Uri(uri, UriKind.Relative)
            };

            return client.SendAsync(request);
        }
    }
}
