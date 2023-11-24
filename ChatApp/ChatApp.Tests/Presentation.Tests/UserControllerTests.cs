using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChatApp.Application.Services.UserContext.Interfaces;
using ChatApp.Application.Services.UserService.Interfaces;
using ChatApp.Domain.DTOs.Http;
using ChatApp.Domain.DTOs.Http.Responses;
using ChatApp.Domain.DTOs.UserDto;
using ChatApp.Domain.Enums;
using ChatApp.Domain.Rooms;
using ChatApp.Domain.Users;
using ChatApp.WebAPI.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace ChatApp.Tests.Presentation.Tests
{
    public class UserControllerTests
    {
        private readonly Mock<IUserService> _userServiceMock;
        private readonly Mock<IUserContext> _userContextMock;
        private readonly UserController _userController;

        public UserControllerTests()
        {
            _userContextMock = new Mock<IUserContext>();
            _userServiceMock = new Mock<IUserService>();
            _userController = new UserController(_userServiceMock.Object, _userContextMock.Object);
        }

        [Theory]
        [InlineData(true, true)] // Successful addition
        [InlineData(false, false)] // Unsuccessful addition
        public async Task AddUserToGroup_ReturnsExpectedResult(bool addUserResult, bool expectOkResult)
        {
            // Arrange
            _userServiceMock.Setup(service => service.AddUserToRoomAsync(It.IsAny<AddUserToRoomDto>()))
                .ReturnsAsync(addUserResult);

            // Act
            var result = await _userController.AddUserToGroup(new AddUserToRoomDto());

            // Assert
            if (expectOkResult)
            {
                var okObjectResult = Assert.IsType<OkObjectResult>(result);
                var addRoomResponseDto = Assert.IsType<AddRoomResponseDto>(okObjectResult.Value);

                Assert.Equal(addUserResult, addRoomResponseDto.WasAdded);
            }
            else
                Assert.IsType<OkObjectResult>(result);

        }

        [Theory]
        [InlineData(true, 1)] // User with groups
        [InlineData(true, 0)] // User with no groups
        [InlineData(false, 0)] // User not found
        public async Task GetUserGroupsAsync_ReturnsExpectedResult(bool userExists, int expectedGroupCount)
        {
            // Arrange
            var userIdClaim = "1";
            _userContextMock.Setup(context => context.GetUserId())
                .Returns(userIdClaim);

            var user = userExists
                ? new User
                {
                    Id = 1,
                    Rooms = expectedGroupCount > 0
                    ? new List<Room>
                    {
                        new Room
                        {
                            Id = 1,
                        }
                    }
                    : new List<Room>()
                }
                : null;

            _userServiceMock.Setup(service => service.GetWithAll(userIdClaim))
                .ReturnsAsync(user);

            // Act
            var result = await _userController.GetUserGroupsAsync();

            // Assert
            if (userExists)
            {
                var okObjectResult = Assert.IsType<OkObjectResult>(result);
                var userGroupsResponseDto = Assert.IsType<UserGroupsResponseDto>(okObjectResult.Value);

                Assert.NotNull(userGroupsResponseDto);
                Assert.Equal(expectedGroupCount, userGroupsResponseDto.GroupsId.Count);
            }
            else
                Assert.IsType<NotFoundResult>(result);
        }

        [Theory]
        [InlineData("user123", typeof(OkObjectResult), true)] // Valid user ID
        [InlineData(null, typeof(BadRequestObjectResult), false)] // Null user ID
        public async Task GetUserAsync_ReturnsExpectedResult(string userIdClaim, Type expectedResponseType,
            bool expectUserRetrieval)
        {
            // Arrange
            _userContextMock.Setup(context => context.GetUserId())
                .Returns(userIdClaim);


            _userServiceMock.Setup(service => service.GetUserAsync(userIdClaim))
                .ReturnsAsync(new User());

            var controller = new UserController(_userServiceMock.Object, _userContextMock.Object);

            // Act
            var result = await controller.GetUserAsync();

            // Assert
            Assert.Equal(expectedResponseType, result?.GetType());

            if (expectUserRetrieval)
            {
                var okObjectResult = Assert.IsType<OkObjectResult>(result);
                var user = Assert.IsType<User>(okObjectResult.Value);

                Assert.NotNull(user);
                _userServiceMock.Verify(service => service.GetUserAsync(userIdClaim), Times.Once);
            }
            else
            {
                var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(result);
                var responseDto = Assert.IsType<ChangePasswordResponseDto>(badRequestObjectResult.Value);

                Assert.NotNull(responseDto);
                Assert.False(responseDto.Success);
                Assert.Equal("Unable to retrieve user id from token.", responseDto.Error);
            }
        }

        [Theory]
        [InlineData(true, typeof(OkObjectResult))] // Successful update
        [InlineData(false, typeof(BadRequestObjectResult))] // Unsuccessful update
        public async Task ChangeUserCredentials_ReturnsExpectedResult(bool updateSuccessful, Type expectedResponseType)
        {
            // Arrange
            _userServiceMock.Setup(service => service.UpdateUserAsync(It.IsAny<UserDto>()))
                .ReturnsAsync(updateSuccessful);

            // Act
            var result = await _userController.ChangeUserCredentials(new UserDto());

            // Assert
            Assert.Equal(expectedResponseType, result.GetType());

            if (updateSuccessful)
            {
                var okObjectResult = Assert.IsType<OkObjectResult>(result);
                var response = Assert.IsType<UpdateUserCredentialResponse>(okObjectResult.Value);

                Assert.NotNull(response);
            }
            else
            {
                var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(result);
                var response = Assert.IsType<UpdateUserCredentialResponse>(badRequestObjectResult.Value);

                Assert.NotNull(response);
            }
        }


        [Fact]
        public async Task GetUsersPage_ReturnsOkResultWithUsers()
        {
            // Arrange

            _userServiceMock.Setup(service => service.GetUsersPageAsync(It.IsAny<GridModelDto<UserColumnsSorting>>()))
                .ReturnsAsync(new GridModelResponse<UserDto>());


            // Act
            var result = await _userController.GetUsersPage(new GridModelDto<UserColumnsSorting>());

            // Assert
            var okObjectResult = Assert.IsType<OkObjectResult>(result);
            var users = Assert.IsType<GridModelResponse<UserDto>> (okObjectResult.Value);

            Assert.NotNull(users);

            _userServiceMock.Verify(service =>
                service.GetUsersPageAsync(It.IsAny<GridModelDto<UserColumnsSorting>>()), Times.Once);
        }
    }
}
