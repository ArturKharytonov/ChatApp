using ChatApp.Domain.Users;
using ChatApp.Persistence.Common.Interfaces;
using Microsoft.AspNetCore.Identity;
using Moq;
using System.Linq.Expressions;
using ChatApp.Domain.DTOs.Http;
using ChatApp.Domain.Rooms;
using ChatApp.Domain.DTOs.UserDto;
using ChatApp.Domain.Enums;
using ChatApp.Tests.Fixtures.Services;
using FluentAssertions;
using ChatApp.Domain.DTOs.Http.Responses.Common;

namespace ChatApp.Tests.Application.Tests
{
    public class UserServiceTests : IClassFixture<UserServiceFixture>
    {

        private readonly UserServiceFixture _fixture;
        public UserServiceTests(UserServiceFixture fixture)
        {
            _fixture = fixture;
        }

        [Theory]
        [InlineData("1")]
        public async Task GetWithAll_ReturnsUserWithMessagesAndRooms(string userId)
        {
            // Arrange
            var user = new User
            {
                Id = int.Parse(userId),
            };

            _fixture.SetupUserRepository(user);


            _fixture.UnitOfWorkMock.Setup(unitOfWork => unitOfWork.GetRepository<User, int>())
                .Returns(_fixture.UserRepository.Object);

            // Act
            var result = await _fixture.UserService.GetWithAll(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(int.Parse(userId), result.Id);

            _fixture.Dispose();
        }

        [Theory]
        [InlineData("1", "2", true)] // User not in the room
        [InlineData("1", "2", false)] // User already in the room
        public async Task AddUserToRoomAsync_ReturnsExpectedResult(
            string userId, string roomId, bool expectedResult)
        {
            // Arrange
            var userToRoomDto = new AddUserToRoomDto
            {
                UserId = userId,
                RoomId = roomId
            };

            var user = new User { Id = 1 };

            var room = new Room { Id = 2, Users = new List<User>() };

            if (!expectedResult)
                room.Users.Add(user);

            _fixture.UnitOfWorkMock
                .Setup(unitOfWork => unitOfWork.GetRepository<Room, int>())
                .Returns(_fixture.RoomRepository.Object);

            _fixture.UserManagerMock
                .Setup(userService =>
                    userService.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(user);

            _fixture.SetupRoomRepository(room);

            // Act
            var result = await _fixture.UserService.AddUserToRoomAsync(userToRoomDto);

            // Assert
            Assert.Equal(expectedResult, result);
            if (expectedResult)
            {
                Assert.Contains(user, room.Users);
                _fixture.UnitOfWorkMock.Verify(uow => uow.SaveAsync(), Times.Once);
            }
            else
                _fixture.UnitOfWorkMock.Verify(uow => uow.SaveAsync(), Times.Never);

            _fixture.Dispose();
        }

        [Theory]
        [InlineData(true, true, true)]
        [InlineData(true, false, false)]
        [InlineData(false, false, false)]
        [InlineData(false, true, false)]
        public async Task ChangePasswordAsync_ValidInput_ReturnsExpectedResult(bool userExists,
            bool passwordMatches, bool expectedResult)
        {
            // Arrange
            _fixture.SetupChangePasswordTest(userExists, passwordMatches);

            // Act
            var result = await _fixture.UserService.ChangePasswordAsync("1", "newPassword", "currentPassword");

            // Assert
            Assert.Equal(expectedResult, result);

            _fixture.Dispose();

        }

        [Theory]
        [InlineData(true, false, true)] // Valid input
        [InlineData(true, true, false)]  // Existing username
        [InlineData(false, false, false)] // Non-existing user
        public async Task UpdateUserAsync_ReturnsExpectedResult(bool userExists,
            bool doesUsernameExist, bool expectedResult)
        {
            // Arrange
            _fixture.SetupUpdateUserTest(userExists, doesUsernameExist);
            var userDto = new UserDto
            {
                Id = 1,
                Username = "newUsername",
                Email = "newEmail@example.com",
                PhoneNumber = "1234567890"
            };

            // Act
            var result = await _fixture.UserService.UpdateUserAsync(userDto);

            // Assert
            Assert.Equal(expectedResult, result);
            _fixture.Dispose();
        }


        [Theory]
        [InlineData("123", UserColumnsSorting.UserName, true, 1)]
        public async Task GetUsersPageAsync_ReturnsExpectedResults(
        string searchTerm, UserColumnsSorting column, bool asc, int pageNumber)
        {
            var data = new GridModelDto<UserColumnsSorting>
            {
                Data = searchTerm,
                Column = column,
                Asc = asc,
                PageNumber = pageNumber,
                Sorting = true
            };
            var users = new List<User>
            {
                new() { Id = 3, UserName = "user5", Email = "user1@example.com", PhoneNumber = "1234567890" },
                new() { Id = 2, UserName = "user2", Email = "user2@example.com", PhoneNumber = "9876543210" },
                new() { Id = 1, UserName = "user1", Email = "user1@example.com", PhoneNumber = "4567890123" }
            }.AsQueryable();

            _fixture.SetupUsersPageService(users);
            // Act
            var result = await _fixture.UserService.GetUsersPageAsync(data);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<GridModelResponse<UserDto>>(result);
            Assert.True(result.Items.Count() <= _fixture.PageSize);
            result.Items.Should().BeInAscendingOrder(p => p.Username);
            _fixture.Dispose();
        }
    }
}

