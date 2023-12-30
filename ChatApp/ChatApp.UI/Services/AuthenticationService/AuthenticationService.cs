using Blazored.LocalStorage;
using ChatApp.Domain.DTOs.Http.Requests.Users;
using ChatApp.Domain.DTOs.Http.Responses.Users;
using ChatApp.UI.Services.CustomAuthenticationState;
using ChatApp.UI.Services.HttpClientPWAService;
using ChatApp.UI.Services.HttpClientPWAService.Interfaces;
using Microsoft.AspNetCore.Components.Authorization;
using IAuthenticationService = ChatApp.UI.Services.AuthenticationService.Interfaces.IAuthenticationService;

namespace ChatApp.UI.Services.AuthenticationService
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

        public async Task<ChangePasswordResponseDto> ChangePasswordAsync(ChangePasswordDto changePasswordDto)
        {
            var result = await _httpClientPwa.PostAsync<ChangePasswordDto, ChangePasswordResponseDto>(HttpClientPwa.ChangePasswordUrl, changePasswordDto);
            return result.Result;
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
            var result =
                await _httpClientPwa.PostAsync<LoginModelDto, LoginResponseDto>(HttpClientPwa.LoginUrl, model);

            if (result.Success && result.Result.Success)
            {
                await _localStorageService.SetItemAsync("token", result.Result.Token);

                ((CustomAuthenticationStateProvider)_authenticationStateProvider).MarkUserAsAuthenticated(result.Result.Token, model.UserName);
            }

            return result.Result;
        }
        public async Task LogoutAsync()
        {
            await _localStorageService.RemoveItemAsync("token");
            ((CustomAuthenticationStateProvider)_authenticationStateProvider).MarkUserAsLoggedOut();
        }
    }
}
