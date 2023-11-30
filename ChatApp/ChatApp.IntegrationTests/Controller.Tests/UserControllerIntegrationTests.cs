using ChatApp.Domain.DTOs.Http.Responses;
using ChatApp.Domain.DTOs.Http;
using ChatApp.Domain.DTOs.UserDto;
using ChatApp.Domain.Enums;
using Newtonsoft.Json;
using System.Net.Http.Json;
using System.Net;
using System.Text;
using ChatApp.Domain.Users;
using FluentAssertions;
using FluentAssertions.Execution;

namespace ChatApp.IntegrationTests.Controller.Tests;

[Collection("Sequential")]
public class UserControllerIntegrationTests : TestBase
{
    private const string _userName = "UserAndRooms";
    private const string _userPassword = "UserAndRooms123!";

    [Fact]
    public async Task AddUserToGroup_ReturnsExpectedStatusCode()
    {
        // Arrange
        var requestDto = new AddUserToRoomDto {RoomId = "3", UserId = "4"};
        var content = new StringContent(JsonConvert.SerializeObject(requestDto), Encoding.UTF8, "application/json");
        await AuthHelper.AddTokenToHeader(_userName, _userPassword);

        // Act
        var response = await Client.PostAsync("/api/user", content);

        // Assert
        using (new AssertionScope())
        {
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var responseBody = await response.Content.ReadFromJsonAsync<AddRoomResponseDto>();
            Assert.NotNull(responseBody);
            Assert.True(responseBody.WasAdded);
        }
    }

    [Fact]
    public async Task GetUserGroupsAsync_ReturnsExpectedResult()
    {
        // Arrange
        await AuthHelper.AddTokenToHeader(_userName, _userPassword);
        // Act
        var response = await Client.GetAsync("/api/user/rooms");

        // Assert
        using (new AssertionScope())
        {
            response.EnsureSuccessStatusCode();
            var responseBody = await response.Content.ReadFromJsonAsync<UserGroupsResponseDto>();
            Assert.NotNull(responseBody);
        }
    }

    [Fact]
    public async Task GetUserAsync_ReturnsExpectedResult()
    {
        //Arrange
        await AuthHelper.AddTokenToHeader(_userName, _userPassword);

        //Act
        var response = await Client.GetAsync("/api/user");

        //Assert
        using (new AssertionScope())
        {
            response.EnsureSuccessStatusCode();
            var responseBody = await response.Content.ReadFromJsonAsync<User>();
            Assert.NotNull(responseBody);
        }
    }

    [Theory]
    [InlineData(true, HttpStatusCode.OK, "afterChangeOfCredentials@test.com")] // Valid update
    [InlineData(false, HttpStatusCode.BadRequest, "example@test.com")] // Invalid update
    public async Task ChangeUserCredentials_ReturnsExpectedStatusCode(bool isValidUpdate, HttpStatusCode expectedStatusCode, string newEmail)
    {
        // Arrange
        var userDto = isValidUpdate 
            ? new UserDto {Id = 5, Username = "UserAndCredentials", Email = "afterChangeOfCredentials@test.com"} 
            : new UserDto { Id = 0, Username = "NotExist", Email = "example@test.com" };
        var content = new StringContent(JsonConvert.SerializeObject(userDto), Encoding.UTF8, "application/json");
        await AuthHelper.AddTokenToHeader(_userName, _userPassword);

        // Act
        var response = await Client.PostAsync("/api/user/credentials", content);

        // Assert
        var wereChanged = await CheckIfRecordExists("AspNetUsers", "Email", newEmail);
        using (new AssertionScope())
        {
            wereChanged.Should().Be(isValidUpdate);
            Assert.Equal(expectedStatusCode, response.StatusCode);
        }
    }

    [Fact]
    public async Task GetUsersPage_ReturnsExpectedResult()
    {
        // Arrange
        var userInput = new GridModelDto<UserColumnsSorting>
        {
            PageNumber = 1,
            Data = "someData",
            Column = UserColumnsSorting.UserName,
            Asc = true,
            Sorting = true
        };
        await AuthHelper.AddTokenToHeader(_userName, _userPassword);
        // Act
        var response = await Client.GetAsync($"/api/user/page?PageNumber={userInput.PageNumber}&Data={userInput.Data}&Column={userInput.Column}" +
                                              $"&Asc={userInput.Asc}&Sorting={userInput.Sorting}");

        // Assert
        using (new AssertionScope())
        {
            response.EnsureSuccessStatusCode();
            var responseBody = await response.Content.ReadFromJsonAsync<GridModelResponse<UserDto>>();
            Assert.NotNull(responseBody);
        }
    }
}