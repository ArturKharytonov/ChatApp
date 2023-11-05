using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Blazored.LocalStorage;
using ChatApp.Application.HttpClientPWAService.Interfaces;
using ChatApp.Domain.DTOs.Http;

namespace ChatApp.Application.HttpClientPWAService
{
    public class HttpClientPwa : IHttpClientPwa
    {
        public const string LoginUrl = "https://localhost:7223/auth/login";
        public const string RegisterUrl = "https://localhost:7223/auth/register";
        public const string ChangePasswordUrl = "https://localhost:7223/api/user/change_password";
        public const string GetUser = "https://localhost:7223/api/user";
        public const string UpdateUser = "https://localhost:7223/api/user/credentials";
        public const string GetUserByCredentials = "https://localhost:7223/api/user/by_credentials";

        private readonly ILocalStorageService _localStorageService;
        public HttpClientPwa(ILocalStorageService localStorageService)
        {
            _localStorageService = localStorageService;
        }

        public async Task<ApiRequestResult<VResult>> PostAsync<TArgument, VResult>(string requestUrl, TArgument data)
        {
            using var httpClient = new HttpClient();

            await TryAddJwtTokenAsync(httpClient);

            var result = await httpClient.PostAsJsonAsync(requestUrl, data);

            return (result.StatusCode.Equals(HttpStatusCode.Unauthorized) && !result.IsSuccessStatusCode)
                ? new ApiRequestResult<VResult>
                {
                    Success = result.IsSuccessStatusCode,
                    Result = default,
                    StatusCode = result.StatusCode
                }
                : new ApiRequestResult<VResult>
                {
                    Success = result.IsSuccessStatusCode,
                    Result = await result.Content.ReadFromJsonAsync<VResult>(),
                    StatusCode = result.StatusCode
                };
        }
        public async Task<ApiRequestResult<T>> GetAsync<T>(string requestUrl, GridModelDto? data = null)
        {
            using var httpClient = new HttpClient();

            await TryAddJwtTokenAsync(httpClient);

            HttpResponseMessage response;
            if (data != null)
                response = await httpClient.GetAsync($"{requestUrl}?data={data.Data}&pageNumber={data.PageNumber}");
            
            else
                response = await httpClient.GetAsync(requestUrl);

            return response.StatusCode.Equals(HttpStatusCode.Unauthorized) && !response.IsSuccessStatusCode
                ? new ApiRequestResult<T>
                {
                    Success = response.IsSuccessStatusCode,
                    Result = default,
                    StatusCode = response.StatusCode
                }
                : new ApiRequestResult<T>
                {
                    Success = response.IsSuccessStatusCode,
                    Result = await response.Content.ReadFromJsonAsync<T>(),
                    StatusCode = response.StatusCode
                };
        }
        private async Task TryAddJwtTokenAsync(HttpClient client)
        {
            var token = await _localStorageService.GetItemAsync<string>("token");

            if (!string.IsNullOrWhiteSpace(token))
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
    }
}
