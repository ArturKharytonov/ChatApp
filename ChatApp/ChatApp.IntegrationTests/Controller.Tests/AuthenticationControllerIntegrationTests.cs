using System.Net;
using System.Net.Http.Json;
using ChatApp.Domain.DTOs.Http;
using Newtonsoft.Json;
using System.Text;
using FluentAssertions;
using FluentAssertions.Execution;
using ChatApp.Domain.DTOs.Http.Responses.Users;

namespace ChatApp.IntegrationTests.Controller.Tests;

[Collection("Sequential")]
public class AuthenticationControllerIntegrationTests : TestBase
{
    [Theory]
    [InlineData("registerUser", "testuser@example.com", "Register123!", true)]
    [InlineData("invalidUser", "invalid@example.com", "InvalidPassword", false)]
    public async Task Register_ValidModel_ReturnsOk(string username, string email, string password, bool expectedSuccess)
    {
        // Arrange
        var registerModel = new RegisterModelDto
        {
            Username = username,
            Email = email,
            Password = password
        };

        var content = new StringContent(JsonConvert.SerializeObject(registerModel), Encoding.UTF8, "application/json");

        // Act
        var response = await Client.PostAsync("/auth/register", content);

        // Assert
        var exist = await CheckIfRecordExists("AspNetUsers", "UserName", username);
        
        using (new AssertionScope())
        {
            exist.Should().Be(expectedSuccess);
            if (expectedSuccess)
            {
                response.EnsureSuccessStatusCode();
                var responseContent = await response.Content.ReadAsStringAsync();
                var responseObject = JsonConvert.DeserializeObject<RegisterResponseDto>(responseContent);
                Assert.Equal(true, responseObject.Successful);
            }
            else
                Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
    }

    [Theory]
    [InlineData("loginUser", "loginUser123!", true)]
    [InlineData("invalidUser", "invalidPassword123!", false)]
    public async Task Login_ReturnsExpectedResult(string userName, string password, bool expectedResult)
    {
        // Arrange
        var loginModelDto = new LoginModelDto
        {
            UserName = userName,
            Password = password
        };

        // Convert the object to JSON
        var jsonContent = new StringContent(JsonConvert.SerializeObject(loginModelDto), Encoding.UTF8, "application/json");

        // Act
        var response = await Client.PostAsync("/auth/login", jsonContent);

        // Assert
        using (new AssertionScope())
        {
            var responseDto = await response.Content.ReadFromJsonAsync<LoginResponseDto>();
            responseDto.Success.Should().Be(expectedResult);

            if (expectedResult)
                response.EnsureSuccessStatusCode();

            else
                response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
    }

    [Theory]
    [InlineData("changePasswordUser123!", "newPassword123!", true)]
    [InlineData("changePasswordUser123!", "newPassword123", false)] // invalid type of new password
    public async Task ChangePassword_ReturnsExpectedResult(string currentPassword, string newPassword, bool expectedResult)
    {
        // Arrange
        var changePasswordDto = new ChangePasswordDto
        {
            CurrentPassword = currentPassword,
            NewPassword = newPassword,
            ConfirmPassword = newPassword
        };

        // Convert the object to JSON
        var jsonContent = new StringContent(JsonConvert.SerializeObject(changePasswordDto), Encoding.UTF8, "application/json");
        await AuthHelper.AddTokenToHeader("changePasswordUser", currentPassword);

        // Act
        var response = await Client.PostAsync("/auth/change_password", jsonContent);

        // Assert
        using (new AssertionScope())
        {
            if (expectedResult)
                response.EnsureSuccessStatusCode();
            else
                response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            var responseDto = await response.Content.ReadFromJsonAsync<ChangePasswordResponseDto>();
            responseDto.Success.Should().Be(expectedResult);
        }
    }
}