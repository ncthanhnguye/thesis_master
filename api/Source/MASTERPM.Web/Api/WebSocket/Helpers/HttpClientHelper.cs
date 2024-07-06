using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace MASTERPM.Web.Api.WebSocket.Helpers
{
    public class HttpClientHelper
    {
        public static async Task<string> MakeRequest<TInput, TOutput>(string baseUrl, string apiUrl, TInput data, string accessToken)
        {
            var contentData = new StringContent(JsonConvert.SerializeObject(data, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() }), Encoding.UTF8, "application/json");

            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", accessToken);

            httpClient.BaseAddress = new Uri(baseUrl);
            httpClient.Timeout = new TimeSpan(0, 0, 2, 0);

            var requestMessage = new HttpRequestMessage
            {
                RequestUri = new Uri(apiUrl, UriKind.Relative),
                Content = contentData,
                Method = HttpMethod.Post
            };

            var response = await httpClient.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);
            return await response.Content.ReadAsStringAsync();
        }
    }
}