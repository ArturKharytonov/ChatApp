using ChatApp.Domain.DTOs.Http;
using System.Net;
using FluentAssertions;
using ChatApp.Domain.Enums;
using FluentAssertions.Execution;

namespace ChatApp.IntegrationTests.Controller.Tests;

[Collection("Sequential")]
public class RoomControllerIntegrationTests : TestBase
{
    private const string _validName = "UserAndRooms";
    private const string _validPassword = "UserAndRooms123!";


    [Fact]
    public async Task GetRooms_ReturnsExpectedResult()
    {
        // Arrange
        await AuthHelper.AddTokenToHeader(_validName, _validPassword);
        var gridModel = new GridModelDto<RoomColumnsSorting>
        {
            PageNumber = 1,
            Data = "room",
            Column = RoomColumnsSorting.Name,
            Asc = true,
            Sorting = true
        };
        // Act
        var response = await Client.GetAsync($"/api/rooms/page?pageNumber={gridModel.PageNumber}&data={gridModel.Data}&column={gridModel.Column}" +
                                              $"&asc={gridModel.Asc}&sorting={gridModel.Sorting}");
        // Assert
        using (new AssertionScope())
            response.EnsureSuccessStatusCode();
    }

    [Theory]
    [InlineData("TestRoom", true)] // Valid room name
    [InlineData("", false)] // Empty room name
    public async Task CreateRoom_ReturnsExpectedResult(string roomName, bool expectSuccess)
    {
        // Arrange
        await AuthHelper.AddTokenToHeader(_validName, _validPassword);

        // Act
        var response = await Client.GetAsync($"/api/rooms/creating?roomName={roomName}");

        //Assert
        var wasAdded = await CheckIfRecordExists("Rooms", "Name", roomName);

        using (new AssertionScope())
        {
            wasAdded.Should().Be(expectSuccess);
            if (expectSuccess)
                response.EnsureSuccessStatusCode();
            else
                Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
    }

    [Theory]
    [InlineData("2", true)] // Valid room ID
    [InlineData("invalidRoomId", false)] // Invalid room ID
    public async Task GetRoom_ReturnsExpectedResult(string roomId, bool expectSuccess)
    {
        // Arrange
        await AuthHelper.AddTokenToHeader(_validName, _validPassword);
        // Act
        var response = await Client.GetAsync($"/api/rooms?id={roomId}");

        // Assert
        using (new AssertionScope())
        {
            if (expectSuccess)
                response.EnsureSuccessStatusCode();

            else
                response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
    }
}