using ChatApp.Domain.DTOs.Http;
using ChatApp.Domain.DTOs.MessageDto;
using ChatApp.Domain.Enums;
using Newtonsoft.Json;
using System.Text;
using System.Web;

namespace ChatApp.IntegrationTests.Controller.Tests;

[Collection("Sequential")]
public class MessageControllerIntegrationTests : IClassFixture<ChatWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly AuthenticationHelper _authHelper;
    private const string _userName = "UserAndMessages";
    private const string _userPassword = "UserAndMessages123!";
    public MessageControllerIntegrationTests(ChatWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
        _authHelper = new AuthenticationHelper(_client);
    }

    [Fact]
    public async Task DeleteMessageAsync_ReturnsOk()
    {
        // Arrange
        var messageId = 1;

        await _authHelper.AddTokenToHeader(_userName, _userPassword);
        // Act
        var response = await _client.DeleteAsync($"/api/message?messageId={messageId}");

        // Assert
        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task GetPageAsync_ReturnsOk()
    {
        // Arrange
        var gridModelDto = new GridModelDto<MessageColumnsSorting>
        {
            PageNumber = 1,
            Data = "hello",
            Column = MessageColumnsSorting.SenderUsername,
            Asc = true,
            Sorting = true
        };

        await _authHelper.AddTokenToHeader(_userName, _userPassword);

        var queryString = GenerateQueryString(gridModelDto);
        var requestUrl = $"/api/message/page?{queryString}";

        // Act
        var response = await _client.GetAsync(requestUrl);

        // Assert
        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task AddMessageAsync_ReturnsOk()
    {
        // Arrange
        var message = new AddMessageDto
        {
            Content = "CreateMessageTest",
            RoomId = 1,
            UserId = "3",
            SentAt = DateTime.Now
        };

        var jsonContent = new StringContent(JsonConvert.SerializeObject(message), Encoding.UTF8, "application/json");

        await _authHelper.AddTokenToHeader(_userName, _userPassword);

        // Act
        var response = await _client.PostAsync("/api/message", jsonContent);

        // Assert
        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task UpdateMessageAsync_ReturnsOk()
    {
        // Arrange
        var messageDto = new MessageDto
        {
            Id = 2,
            Content = "Hello, this is updated message.",
            SentAt = DateTime.Now,
            RoomName = "roomForMessages",
            SenderUsername = "UserAndMessages"
        };
        var jsonContent = new StringContent(JsonConvert.SerializeObject(messageDto), Encoding.UTF8, "application/json");
        await _authHelper.AddTokenToHeader(_userName, _userPassword);

        // Act
        var response = await _client.PutAsync("/api/message", jsonContent);

        // Assert
        response.EnsureSuccessStatusCode();
    }
    private static string GenerateQueryString(GridModelDto<MessageColumnsSorting> gridModelDto)
    {
        var queryParameters = new System.Collections.Specialized.NameValueCollection
        {
            ["data"] = gridModelDto.Data,
            ["pageNumber"] = gridModelDto.PageNumber.ToString(),
            ["column"] = gridModelDto.Column.ToString(),
            ["asc"] = gridModelDto.Asc.ToString(),
            ["sorting"] = gridModelDto.Sorting.ToString()
        };

        var queryString = string.Join("&",
            queryParameters.AllKeys.Select(key =>
                $"{HttpUtility.UrlEncode(key)}={HttpUtility.UrlEncode(queryParameters[key])}"
            )
        );

        return queryString;
    }
}