using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using ChatApp.Application.HttpClientPWAService.Interfaces;
using Microsoft.AspNetCore.Components.Authorization;

namespace ChatApp.Application.CustomAuthenticationState
{
    public class CustomAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly IHttpClientPwa _httpClientPwa;
        private readonly ILocalStorageService _localStorage;
        public CustomAuthenticationStateProvider(IHttpClientPwa httpClientPwa, ILocalStorageService localStorage)
        {
            _httpClientPwa = httpClientPwa;
            _localStorage = localStorage;
        }

        public void MarkUserAsAuthenticated(string username)
        {
            var authenticatedUser =
                new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.NameIdentifier, username) }, "apiauth"));
            var authState = Task.FromResult(new AuthenticationState(authenticatedUser));
            NotifyAuthenticationStateChanged(authState);
        }

        public void MarkUserAsLoggedOut()
        {
            var anonymousUser =
                new ClaimsPrincipal(new ClaimsIdentity());
            var authState = Task.FromResult(new AuthenticationState(anonymousUser));
            NotifyAuthenticationStateChanged(authState);
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var savedToken = await _localStorage.GetItemAsync<string>("token");
            if (string.IsNullOrEmpty(savedToken))
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            _httpClientPwa.SetAuthorizationHeader(new AuthenticationHeaderValue("bearer", savedToken));
            return new AuthenticationState(
                new ClaimsPrincipal(new ClaimsIdentity(ParseClaimsFromJwt(savedToken), "jwt")));
        }
        private IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
        {
            var claims = new List<Claim>();
            var payload = jwt.Split('.')[1];
            var jsonBytes = ParseBase64WithoutPadding(payload);
            var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);

            keyValuePairs!.TryGetValue(ClaimTypes.Role, out object roles);

            if (roles != null)
            {
                if (roles.ToString().Trim().StartsWith("["))
                {
                    var parsedRoles = JsonSerializer.Deserialize<string[]>(roles.ToString());

                    claims.AddRange(parsedRoles
                        .Select(parsedRole => new Claim(ClaimTypes.Role, parsedRole)));
                }
                else
                    claims.Add(new Claim(ClaimTypes.Role, roles.ToString()));
            }

            keyValuePairs.Remove(ClaimTypes.Role);

            return claims;
        }

        private byte[] ParseBase64WithoutPadding(string base64)
        {
            switch (base64.Length % 4)
            {
                case 2: base64 += "=="; break;
                case 3: base64 += "="; break;
            }
            return Convert.FromBase64String(base64);
        }
    }
}
