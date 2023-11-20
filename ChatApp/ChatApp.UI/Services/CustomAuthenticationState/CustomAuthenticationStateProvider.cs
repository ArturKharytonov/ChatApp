using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json;
using Blazored.LocalStorage;
using ChatApp.UI.Services.HttpClientPWAService.Interfaces;
using Microsoft.AspNetCore.Components.Authorization;

namespace ChatApp.UI.Services.CustomAuthenticationState
{
    public class CustomAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly ILocalStorageService _localStorage;
        private readonly IHttpClientPwa _httpClientPwa;
        public CustomAuthenticationStateProvider(ILocalStorageService localStorage, IHttpClientPwa httpClientPwa)
        {
            _localStorage = localStorage;
            _httpClientPwa = httpClientPwa;
        }

        public void MarkUserAsAuthenticated(string token, string username)
        {
            _httpClientPwa.TryAddJwtToken(token);
            var claims = ParseJwtToClaims(token);
            var authenticatedUser = new ClaimsPrincipal(new ClaimsIdentity(claims, "JwtAuth"));
            var authState = Task.FromResult(new AuthenticationState(authenticatedUser));

            NotifyAuthenticationStateChanged(authState);
        }
        public void MarkUserAsLoggedOut()
        {
            _httpClientPwa.DeleteJwtToken();
            var anonymousUser =
                new ClaimsPrincipal(new ClaimsIdentity());
            var authState = Task.FromResult(new AuthenticationState(anonymousUser));
            NotifyAuthenticationStateChanged(authState);
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var jwtToken = await _localStorage.GetItemAsync<string>("token");

            var isTokenValid = IsTokenValid(jwtToken);
            var authState = new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));

            if (isTokenValid)
            {
                _httpClientPwa.TryAddJwtToken(jwtToken);

                authState = new AuthenticationState(
                    new ClaimsPrincipal(
                        new ClaimsIdentity(ParseJwtToClaims(jwtToken), "JwtAuth"
                        )));
            }
            else
            {
                await _localStorage.RemoveItemAsync("token");
                _httpClientPwa.DeleteJwtToken();
            }

            NotifyAuthenticationStateChanged(Task.FromResult(authState));

            return authState;
        }
        private bool IsTokenValid(string jwtToken)
        {
            if (string.IsNullOrEmpty(jwtToken))
                return false;

            JwtSecurityToken jwtSecurity;
            try
            {
                jwtSecurity = new JwtSecurityToken(jwtToken);
            }
            catch (Exception) { return false; }

            return jwtSecurity.ValidTo > DateTime.UtcNow;
        }
        private IEnumerable<Claim> ParseJwtToClaims(string jwtToken)
        {
            var payload = jwtToken.Split('.')[1];
            var jsonBytes = ParseBase64WithoutPadding(payload);
            var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);
            return keyValuePairs.Select(kvp => new Claim(kvp.Key, kvp.Value.ToString()));
        }
        private byte[] ParseBase64WithoutPadding(string base64)
        {
            switch (base64.Length % 4)
            {
                case 2:
                    base64 += "==";
                    break;
                case 3:
                    base64 += "=";
                    break;
            }
            return Convert.FromBase64String(base64);
        }
    }
}
