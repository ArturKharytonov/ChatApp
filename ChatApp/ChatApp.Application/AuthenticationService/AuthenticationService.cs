using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Blazored.LocalStorage;
using ChatApp.Application.AuthenticationService.Interfaces;
using ChatApp.Application.CustomAuthenticationState;
using ChatApp.Application.HttpClientPWAService.Interfaces;
using ChatApp.UI.ViewModels;
using ChatApp.UI.ViewModels.Responses;
using Microsoft.AspNetCore.Components.Authorization;

namespace ChatApp.Application.AuthenticationService
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IHttpClientPwa _httpClientPwa;
        private readonly AuthenticationStateProvider _authenticationStateProvider;
        private readonly ILocalStorageService _localStorageService;
        public AuthenticationService(IHttpClientPwa httpClientPwa, AuthenticationStateProvider authenticationStateProvider, ILocalStorageService localStorageService)
        {
            _httpClientPwa = httpClientPwa;
            _authenticationStateProvider = authenticationStateProvider;
            _localStorageService = localStorageService;
        }

        public async Task<RegisterResponse> RegisterAsync(RegisterModel model)
        {
            var result = await _httpClientPwa.PostAsJsonAsync("auth/register", model);
            return !result.IsSuccessStatusCode
                ? new RegisterResponse { Successful = false, Errors = new List<string> { "Error occurred" } }
                : new RegisterResponse { Successful = true };
        }

        public async Task<LoginResponse> LoginAsync(LoginModel model)
        {
            var loginAsJson = JsonSerializer.Serialize(model);
            var response = await _httpClientPwa.PostAsync("auth/login",
                new StringContent(loginAsJson, Encoding.UTF8, "applications/json"));

            var loginResult = JsonSerializer.Deserialize<LoginResponse>(await response.Content.ReadAsStringAsync(),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            if (!response.IsSuccessStatusCode)
                return loginResult;
            await _localStorageService.SetItemAsync("token", loginResult.Token);
            ((CustomAuthenticationStateProvider)_authenticationStateProvider).MarkUserAsAuthenticated(model.Username);

            _httpClientPwa.SetAuthorizationHeader(new AuthenticationHeaderValue("bearer", loginResult.Token));
            return loginResult;
        }

        public async Task LogoutAsync()
        {
            await _localStorageService.RemoveItemAsync("token");
            ((CustomAuthenticationStateProvider)_authenticationStateProvider).MarkUserAsLoggedOut();

            _httpClientPwa.SetAuthorizationHeader();
        }
    }
}
