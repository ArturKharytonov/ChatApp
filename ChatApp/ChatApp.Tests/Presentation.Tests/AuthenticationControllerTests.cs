using ChatApp.Application.Services.JwtHandler.Interfaces;
using ChatApp.Application.Services.UserContext.Interfaces;
using ChatApp.Application.Services.UserService.Interfaces;
using ChatApp.Domain.DTOs.Http.Responses;
using ChatApp.Domain.DTOs.Http;
using ChatApp.Domain.Users;
using ChatApp.Tests.Fixtures.Controllers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace ChatApp.Tests.Presentation.Tests
{
    public class AuthenticationControllerTests : IClassFixture<AuthenticationControllerFixture>
    {
        private readonly AuthenticationControllerFixture _fixture;
        public AuthenticationControllerTests()
        {
            _fixture = new AuthenticationControllerFixture();
        }

        [Theory]
        [InlineData("1", "oldPassword", "newPassword", true)]
        [InlineData("2", "wrongPassword", "newPassword", false)]
        public async Task ChangePasswordAsync_ReturnsCorrectResult(string userId, string currentPassword, string newPassword, bool changePasswordResult)
        {
            // Arrange
            var changePasswordDto = new ChangePasswordDto { CurrentPassword = currentPassword, NewPassword = newPassword };

            _fixture.UserContextMock
                .Setup(uc => uc.GetUserId())
                .Returns(userId);

            _fixture.UserServiceMock
                .Setup(us => us.ChangePasswordAsync(userId, changePasswordDto.NewPassword, changePasswordDto.CurrentPassword))
                .ReturnsAsync(changePasswordResult);

            // Act
            var result = await _fixture.AuthenticationController.ChangePasswordAsync(changePasswordDto);

            // Assert
            if (changePasswordResult)
            {
                var okResult = Assert.IsType<OkObjectResult>(result);
                var responseDto = Assert.IsType<ChangePasswordResponseDto>(okResult.Value);
                Assert.Equal(changePasswordResult, responseDto.Success);
            }
            else
            {
                var badRequest = Assert.IsType<BadRequestObjectResult>(result);
                var responseDto = Assert.IsType<ChangePasswordResponseDto>(badRequest.Value);
                Assert.False(responseDto.Success);
            }
        }

        [Theory]
        [InlineData("username", "password", true)]
        [InlineData("invalidUser", "password", false)]
        public async Task LoginAsync_ReturnsCorrectResult(string userName, string password, bool loginResult)
        {
            // Arrange
            var loginModelDto = new LoginModelDto { UserName = userName, Password = password };
            var user = loginResult ? new User { Id = 1, UserName = userName } : null;

            _fixture.UserManagerMock
                .Setup(um => um.FindByNameAsync(loginModelDto.UserName))
                .ReturnsAsync(user);

            _fixture.UserManagerMock
                .Setup(um => um.CheckPasswordAsync(user, loginModelDto.Password))
                .ReturnsAsync(loginResult);

            if (loginResult)
            {
                _fixture.JwtServiceMock
                    .Setup(jwt => jwt.GetToken(user.Id, loginModelDto.UserName))
                    .Returns("token");
            }

            // Act
            var result = await _fixture.AuthenticationController.LoginAsync(loginModelDto);

            // Assert
            if (loginResult)
            {
                var okResult = Assert.IsType<OkObjectResult>(result);
                var responseDto = Assert.IsType<LoginResponseDto>(okResult.Value);
                Assert.NotNull(responseDto.Token);
                Assert.True(responseDto.Success);
            }
            else
            {
                var badRequest = Assert.IsType<BadRequestObjectResult>(result);
                var responseDto = Assert.IsType<LoginResponseDto>(badRequest.Value);
                Assert.Null(responseDto.Token);
                Assert.False(responseDto.Success);
            }
        }

        [Theory]
        [InlineData("newuser", "newuser@example.com", "password", true)]
        [InlineData("existingUser", "existingUser@example.com", "password", false)]
        public async Task RegisterAsync_ReturnsCorrectResult(string username, string email, string password, bool registrationResult)
        {
            // Arrange
            var registerModelDto = new RegisterModelDto { Username = username, Email = email, Password = password };
            var userResult = registrationResult ? IdentityResult.Success : IdentityResult.Failed();

            _fixture.UserManagerMock.Setup(um => um.CreateAsync(It.IsAny<User>(), registerModelDto.Password)).ReturnsAsync(userResult);

            // Act
            var result = await _fixture.AuthenticationController.RegisterAsync(registerModelDto);

            // Assert
            if (registrationResult)
            {
                var okResult = Assert.IsType<OkObjectResult>(result);
                var responseDto = Assert.IsType<RegisterResponseDto>(okResult.Value);
                Assert.True(responseDto.Successful);
            }
            else
            {
                var badRequest = Assert.IsType<BadRequestObjectResult>(result);
                var responseDto = Assert.IsType<RegisterResponseDto>(badRequest.Value);
                Assert.False(responseDto.Successful);
                Assert.NotNull(responseDto.Errors);
            }
        }
    }
}
