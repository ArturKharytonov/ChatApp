using ChatApp.Domain.DTOs.Http.Responses;
using ChatApp.Domain.DTOs.Http;
using Newtonsoft.Json;
using System.Net.Http.Json;
using System.Text;

namespace ChatApp.IntegrationTests;

public class AuthenticationHelper
{
    private readonly HttpClient _client;

    public AuthenticationHelper(HttpClient client)
    {
        _client = client;
    }

    public async Task AddTokenToHeader(string userName, string password)
    {
        var authToken = await GetAuthTokenAsync(userName, password);
        _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {authToken ?? string.Empty}");
    }

    private async Task<string?> GetAuthTokenAsync(string userName, string password)
    {
        var loginModelDto = new LoginModelDto
        {
            UserName = userName,
            Password = password
        };

        var jsonContent = new StringContent(JsonConvert.SerializeObject(loginModelDto), Encoding.UTF8, "application/json");

        var response = await _client.PostAsync("/auth/login", jsonContent);

        var responseDto = await response.Content.ReadFromJsonAsync<LoginResponseDto>();
        return responseDto?.Token;
    }
}