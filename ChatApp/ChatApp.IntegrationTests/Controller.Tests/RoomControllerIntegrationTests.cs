using ChatApp.Domain.DTOs.Http.Responses;
using ChatApp.Domain.DTOs.Http;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using FluentAssertions;
using ChatApp.Domain.Enums;
using Microsoft.Extensions.Configuration.UserSecrets;

namespace ChatApp.IntegrationTests.Controller.Tests;

[Collection("Sequential")]
public class RoomControllerIntegrationTests : IClassFixture<ChatWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly AuthenticationHelper _authHelper;
    private const string _validName = "UserAndRooms";
    private const string _validPassword = "UserAndRooms123!";

    public RoomControllerIntegrationTests(ChatWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
        _authHelper = new AuthenticationHelper(_client);
    }

    [Fact]
    public async Task GetRooms_ReturnsExpectedResult()
    {
        // Arrange
        await _authHelper.AddTokenToHeader(_validName, _validPassword);
        var gridModel = new GridModelDto<RoomColumnsSorting>
        {
            PageNumber = 1,
            Data = "room",
            Column = RoomColumnsSorting.Name,
            Asc = true,
            Sorting = true
        };
        // Act
        var response = await _client.GetAsync($"/api/rooms/page?pageNumber={gridModel.PageNumber}&data={gridModel.Data}&column={gridModel.Column}" +
                                              $"&asc={gridModel.Asc}&sorting={gridModel.Sorting}");
        // Assert
        response.EnsureSuccessStatusCode();
    }

    [Theory]
    [InlineData("TestRoom", true)] // Valid room name
    [InlineData("", false)] // Empty room name
    public async Task CreateRoom_ReturnsExpectedResult(string roomName, bool expectSuccess)
    {
        // Arrange
        await _authHelper.AddTokenToHeader(_validName, _validPassword);
        // Act
        var response = await _client.GetAsync($"/api/rooms/creating?roomName={roomName}");

        //Assert
        if (expectSuccess)
            response.EnsureSuccessStatusCode();
        
        else
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Theory]
    [InlineData("2", true)] // Valid room ID
    [InlineData("invalidRoomId", false)] // Invalid room ID
    public async Task GetRoom_ReturnsExpectedResult(string roomId, bool expectSuccess)
    {
        // Arrange
        await _authHelper.AddTokenToHeader(_validName, _validPassword);
        // Act
        var response = await _client.GetAsync($"/api/rooms?id={roomId}");

        // Assert
        if(expectSuccess)
            response.EnsureSuccessStatusCode();
        
        else
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

    }
}