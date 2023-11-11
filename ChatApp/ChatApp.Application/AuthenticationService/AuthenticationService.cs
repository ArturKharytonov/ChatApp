using Blazored.LocalStorage;
using ChatApp.Application.AuthenticationService.Interfaces;
using ChatApp.Application.CustomAuthenticationState;
using ChatApp.Application.HttpClientPWAService;
using ChatApp.Application.HttpClientPWAService.Interfaces;
using ChatApp.Domain.DTOs.Http;
using ChatApp.Domain.DTOs.Http.Responses;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Principal;
using System.Text;

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
