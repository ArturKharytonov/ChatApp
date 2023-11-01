using Blazored.LocalStorage;
using ChatApp.Application.AuthenticationService.Interfaces;
using ChatApp.Application.CustomAuthenticationState;
using ChatApp.Application.HttpClientPWAService;
using ChatApp.Application.HttpClientPWAService.Interfaces;
using ChatApp.Domain.DTOs.Http;
using ChatApp.Domain.DTOs.Http.Responses;
using Microsoft.AspNetCore.Components.Authorization;

namespace ChatApp.Application.AuthenticationService
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly AuthenticationStateProvider _authenticationStateProvider;
        private readonly ILocalStorageService _localStorageService;
        private readonly IHttpClientPwa _httpClientPwa;
        public AuthenticationService(AuthenticationStateProvider authenticationStateProvider, ILocalStorageService localStorageService, IHttpClientPwa httpClientPwa)
        {
            _authenticationStateProvider = authenticationStateProvider;
            _localStorageService = localStorageService;
            _httpClientPwa = httpClientPwa;
        }

        public async Task<RegisterResponseDto> RegisterAsync(RegisterModelDto model)
        {
            var result = await _httpClientPwa.PostAsync<RegisterModelDto, RegisterResponseDto>(HttpClientPwa.RegisterUrl, model);

            return !result.Success
                ? new RegisterResponseDto { Successful = false, Errors = result.Result?.Errors ?? new List<string> {"Error occurred"} }
                : new RegisterResponseDto { Successful = true };
        }

        public async Task<LoginResponseDto> LoginAsync(LoginModelDto model)
        {
            var result = await _httpClientPwa.PostAsync<LoginModelDto, LoginResponseDto>(HttpClientPwa.LoginUrl, model);

            if (result.Success && result.Result.Success)
            {
                await _localStorageService.SetItemAsync("token", result.Result.Token);

                ((CustomAuthenticationStateProvider)_authenticationStateProvider).MarkUserAsAuthenticated(model.UserName);
                //HttpClientPp.SetAuthorizationHeader(new AuthenticationHeaderValue("bearer", loginResult.Token));
            }

            return result.Result;
        }

        public async Task LogoutAsync()
        {
            await _localStorageService.RemoveItemAsync("token");
            ((CustomAuthenticationStateProvider)_authenticationStateProvider).MarkUserAsLoggedOut();

            //_httpClientPwa.SetAuthorizationHeader();
        }
    }
}
