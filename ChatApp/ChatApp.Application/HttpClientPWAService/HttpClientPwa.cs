using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Blazored.LocalStorage;
using ChatApp.Application.HttpClientPWAService.Interfaces;
using ChatApp.Domain.DTOs.Http;
using ChatApp.Domain.DTOs.Http.Responses;

namespace ChatApp.Application.HttpClientPWAService
{
    public class HttpClientPwa : IHttpClientPwa
    {
        public const string LoginUrl = "https://localhost:7223/auth/login";
        public const string RegisterUrl = "https://localhost:7223/auth/register";
        public const string ChangePasswordUrl = "https://localhost:7223/api/user/change_password";

        private readonly ILocalStorageService _localStorageService;
        public HttpClientPwa(ILocalStorageService localStorageService)
        {
            _localStorageService = localStorageService;
        }
        public async Task<ApiRequestResult<VResult>> PostAsync<TArgument, VResult>(string requestUrl, TArgument data, bool withJwt = false)
        {
            using var httpClient = new HttpClient();
            if (withJwt)
            {
                httpClient.DefaultRequestHeaders.Authorization = 
                    new AuthenticationHeaderValue("Bearer", await _localStorageService.GetItemAsync<string>("token"));
            }

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
    }
}
