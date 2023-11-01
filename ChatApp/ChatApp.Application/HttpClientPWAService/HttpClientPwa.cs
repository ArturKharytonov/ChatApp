using System.Net.Http.Headers;
using System.Net.Http.Json;
using ChatApp.Application.HttpClientPWAService.Interfaces;

namespace ChatApp.Application.HttpClientPWAService
{
    public class HttpClientPwa : IHttpClientPwa
    {
        public const string LoginUrl = "https://localhost:7223/auth/login";
        public const string RegisterUrl = "https://localhost:7223/auth/register";

        public void SetAuthorizationHeader(AuthenticationHeaderValue? header = null)
        {
            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = header;
        }

        public async Task<ApiRequestResult<TResult>> PostAsync<TArgument, TResult>(string requestUrl, TArgument data)
        {
            using var httpClient = new HttpClient();
            var result = await httpClient.PostAsJsonAsync(requestUrl, data);

            return new ApiRequestResult<TResult>()
            {
                Success = result.IsSuccessStatusCode,
                Result = await result.Content.ReadFromJsonAsync<TResult>(),
                StatusCode = result.StatusCode
            };
        }
    }
}
