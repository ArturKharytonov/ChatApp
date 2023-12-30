using ChatApp.Domain.DTOs.Http;
using ChatApp.Domain.DTOs.Http.Requests.Common;
using ChatApp.Domain.DTOs.MessageDto;
using ChatApp.Domain.Enums;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.Extensions.Configuration.UserSecrets;
using Newtonsoft.Json;
using System.Text;
using System.Web;
using Xunit.Sdk;

namespace ChatApp.IntegrationTests.Controller.Tests;

[Collection("Sequential")]
public class MessageControllerIntegrationTests : TestBase
{
    private const string _userName = "UserAndMessages";
    private const string _userPassword = "UserAndMessages123!";

    [Fact]
    public async Task GetAllMessagesAsync_ReturnsOk()
    {
        // Arrange
        var roomId = 1;
        await AuthHelper.AddTokenToHeader(_userName, _userPassword);

        // Act
        var response = await Client.GetAsync($"/api/message/all/{roomId}");

        // Assert
        response.EnsureSuccessStatusCode();
    }
    [Fact]
    public async Task DeleteMessageAsync_ReturnsOk()
    {
        // Arrange
        var messageId = 1;
        await AuthHelper.AddTokenToHeader(_userName, _userPassword);

        // Act
        var response = await Client.DeleteAsync($"/api/message?messageId={messageId}");

        // Assert
        var messageExists = await CheckIfRecordExists("Messages", "Id", messageId);
        using (new AssertionScope())
        {
            response.EnsureSuccessStatusCode();
            messageExists.Should().BeFalse();
        }
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

        await AuthHelper.AddTokenToHeader(_userName, _userPassword);

        var queryString = GenerateQueryString(gridModelDto);
        var requestUrl = $"/api/message/page?{queryString}";

        // Act
        var response = await Client.GetAsync(requestUrl);

        // Assert
        using (new AssertionScope())
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

        await AuthHelper.AddTokenToHeader(_userName, _userPassword);

        // Act
        var response = await Client.PostAsync("/api/message", jsonContent);

        // Assert
        using (new AssertionScope())
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
        await AuthHelper.AddTokenToHeader(_userName, _userPassword);

        // Act
        var response = await Client.PutAsync("/api/message", jsonContent);

        // Assert
        var messageWithOldContentExists = await CheckIfRecordExists("Messages", "Content", "Hello world.");
        var messageWithNewContentExists = await CheckIfRecordExists("Messages", "Content", "Hello, this is updated message.");

        using (new AssertionScope())
        {
            response.EnsureSuccessStatusCode();
            messageWithNewContentExists.Should().BeTrue();
            messageWithOldContentExists.Should().BeFalse();
        }
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