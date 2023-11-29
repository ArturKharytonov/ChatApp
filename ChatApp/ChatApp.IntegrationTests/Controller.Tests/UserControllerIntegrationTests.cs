using ChatApp.Domain.DTOs.Http.Responses;
using ChatApp.Domain.DTOs.Http;
using ChatApp.Domain.DTOs.UserDto;
using ChatApp.Domain.Enums;
using Newtonsoft.Json;
using System.Net.Http.Json;
using System.Net;
using System.Text;
using ChatApp.Domain.Users;

namespace ChatApp.IntegrationTests.Controller.Tests;

[Collection("Sequential")]
public class UserControllerIntegrationTests : IClassFixture<ChatWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly AuthenticationHelper _authHelper;
    private const string _userName = "UserAndRooms";
    private const string _userPassword = "UserAndRooms123!";
    public UserControllerIntegrationTests(ChatWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
        _authHelper = new AuthenticationHelper(_client);
    }
    [Fact]
    public async Task AddUserToGroup_ReturnsExpectedStatusCode()
    {
        // Arrange
        var requestDto = new AddUserToRoomDto {RoomId = "3", UserId = "4"};
        var content = new StringContent(JsonConvert.SerializeObject(requestDto), Encoding.UTF8, "application/json");
        await _authHelper.AddTokenToHeader(_userName, _userPassword);

        // Act
        var response = await _client.PostAsync("/api/user", content);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var responseBody = await response.Content.ReadFromJsonAsync<AddRoomResponseDto>();
        Assert.NotNull(responseBody);
        Assert.True(responseBody.WasAdded);
    }

    [Fact]
    public async Task GetUserGroupsAsync_ReturnsExpectedResult()
    {
        // Arrange
        await _authHelper.AddTokenToHeader(_userName, _userPassword);
        // Act
        var response = await _client.GetAsync("/api/user/rooms");

        // Assert
        response.EnsureSuccessStatusCode();
        var responseBody = await response.Content.ReadFromJsonAsync<UserGroupsResponseDto>();
        Assert.NotNull(responseBody);
    }

    [Fact]
    public async Task GetUserAsync_ReturnsExpectedResult()
    {
        //Arrange
        await _authHelper.AddTokenToHeader(_userName, _userPassword);

        //Act
        var response = await _client.GetAsync("/api/user");

        //Assert
        response.EnsureSuccessStatusCode();
        var responseBody = await response.Content.ReadFromJsonAsync<User>();
        Assert.NotNull(responseBody);
    }

    [Theory]
    [InlineData(true, HttpStatusCode.OK)] // Valid update
    [InlineData(false, HttpStatusCode.BadRequest)] // Invalid update
    public async Task ChangeUserCredentials_ReturnsExpectedStatusCode(bool isValidUpdate, HttpStatusCode expectedStatusCode)
    {
        // Arrange
        var userDto = isValidUpdate 
            ? new UserDto {Id = 5, Username = "UserAndCredentials", Email = "example@test.com"} 
            : new UserDto { Id = 0, Username = "NotExist", Email = "example@test.com" };
        var content = new StringContent(JsonConvert.SerializeObject(userDto), Encoding.UTF8, "application/json");
        await _authHelper.AddTokenToHeader(_userName, _userPassword);

        // Act
        var response = await _client.PostAsync("/api/user/credentials", content);

        // Assert
        Assert.Equal(expectedStatusCode, response.StatusCode);

        if (expectedStatusCode == HttpStatusCode.OK)
        {
            var responseBody = await response.Content.ReadFromJsonAsync<UpdateUserCredentialResponse>();
            Assert.NotNull(responseBody);
        }
        else
        {
            var responseBody = await response.Content.ReadFromJsonAsync<UpdateUserCredentialResponse>();
            Assert.NotNull(responseBody);
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
        await _authHelper.AddTokenToHeader(_userName, _userPassword);
        // Act
        var response = await _client.GetAsync($"/api/user/page?PageNumber={userInput.PageNumber}&Data={userInput.Data}&Column={userInput.Column}" +
                                              $"&Asc={userInput.Asc}&Sorting={userInput.Sorting}");

        // Assert
        response.EnsureSuccessStatusCode();
        var responseBody = await response.Content.ReadFromJsonAsync<GridModelResponse<UserDto>>();
        Assert.NotNull(responseBody);
    }
}