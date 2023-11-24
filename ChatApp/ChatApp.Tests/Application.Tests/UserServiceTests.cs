using ChatApp.Application.Services.QueryBuilder.Interfaces;
using ChatApp.Application.Services.UserService;
using ChatApp.Domain.Users;
using ChatApp.Persistence.Common.Interfaces;
using ChatApp.Persistence.UnitOfWork.Interfaces;
using Microsoft.AspNetCore.Identity;
using Moq;
using System.Linq.Expressions;
using ChatApp.Domain.DTOs.Http;
using ChatApp.Domain.Rooms;
using ChatApp.Domain.DTOs.UserDto;
using ChatApp.Domain.Enums;
using ChatApp.Domain.DTOs.Http.Responses;
using Microsoft.EntityFrameworkCore;
using System;

namespace ChatApp.Tests.Application.Tests
{
    public class UserServiceTests
    {
        private readonly Mock<UserManager<User>> _userManagerMock;
        private readonly Mock<IQueryBuilder<User>> _queryBuilderMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IRepository<User, int>> _userRepository;
        private readonly UserService _userService;
        private const int _pageSize = 5;
        public UserServiceTests()
        {
            _userManagerMock = new Mock<UserManager<User>>(
                Mock.Of<IUserStore<User>>(),
                null, null, null, null, null, null, null, null);
            _queryBuilderMock = new Mock<IQueryBuilder<User>>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _userRepository = new Mock<IRepository<User, int>>();
            _userService = new UserService(_userManagerMock.Object, _queryBuilderMock.Object, _unitOfWorkMock.Object);
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

            _userRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<int>(),
                    It.IsAny<Expression<Func<User, object>>[]>()))
                .ReturnsAsync(user);

            _unitOfWorkMock.Setup(unitOfWork => unitOfWork.GetRepository<User, int>())
                .Returns(_userRepository.Object);

            // Act
            var result = await _userService.GetWithAll(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(int.Parse(userId), result.Id);
        }

        [Theory]
        [InlineData("1", "2", true)] // User not in the room
        [InlineData("1", "2", false)] // User already in the room
        public async Task AddUserToRoomAsync_ReturnsExpectedResult(
            string userId, string roomId, bool expectedResult)
        {
            // Arrange
            var roomRepositoryMock = new Mock<IRepository<Room, int>>();

            var userToRoomDto = new AddUserToRoomDto
            {
                UserId = userId,
                RoomId = roomId
            };

            var user = new User
            {
                Id = 1,
            };

            var room = new Room
            {
                Id = 2,
                Users = new List<User>()
            };

            if (!expectedResult)
                room.Users.Add(user);
            

            _unitOfWorkMock.Setup(unitOfWork => unitOfWork.GetRepository<Room, int>())
                .Returns(roomRepositoryMock.Object);

            _userManagerMock.Setup(userService => userService.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(user);

            roomRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<int>(), It.IsAny<Expression<Func<Room, object>>[]>()))
                .ReturnsAsync(room);

            // Act
            var result = await _userService.AddUserToRoomAsync(userToRoomDto);

            // Assert
            Assert.Equal(expectedResult, result);
            if (expectedResult)
            {
                Assert.Contains(user, room.Users);
                _unitOfWorkMock.Verify(uow => uow.SaveAsync(), Times.Once);
            }
            else
                _unitOfWorkMock.Verify(uow => uow.SaveAsync(), Times.Never);
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
            var user = userExists ? new User { Id = 1 } : null;
            _userManagerMock
                .Setup(u => u.FindByIdAsync("1"))
                .ReturnsAsync(user);
            _userManagerMock
                .Setup(u => u.CheckPasswordAsync(user, "currentPassword"))
                .ReturnsAsync(passwordMatches);
            _userManagerMock
                .Setup(u => u.GeneratePasswordResetTokenAsync(user))
                .ReturnsAsync("token");
            _userManagerMock
                .Setup(u => u.ResetPasswordAsync(user, "token", "newPassword"))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _userService.ChangePasswordAsync("1", "newPassword", "currentPassword");

            // Assert
            Assert.Equal(expectedResult, result);
        }

        [Theory]
        [InlineData(true, false, true)] // Valid input
        [InlineData(true, true, false)]  // Existing username
        [InlineData(false, false, false)] // Non-existing user
        public async Task UpdateUserAsync_ReturnsExpectedResult(bool userExists,
            bool doesUsernameExist, bool expectedResult)
        {
            // Arrange
            var existingUser = userExists ? new User { Id = 1, UserName = "existingUsername" } : null;

            _userManagerMock
                .Setup(u => u.FindByIdAsync("1"))
                .ReturnsAsync(existingUser);

            _userManagerMock
                .Setup(u => u.FindByNameAsync(It.IsAny<string>()))
                .ReturnsAsync(doesUsernameExist ? new User() : null);

            _userManagerMock
                .Setup(u => u.UpdateAsync(existingUser))
                .ReturnsAsync(IdentityResult.Success);

            var userDto = new UserDto
            {
                Id = 1,
                Username = "newUsername",
                Email = "newEmail@example.com",
                PhoneNumber = "1234567890"
            };

            // Act
            var result = await _userService.UpdateUserAsync(userDto);

            // Assert
            Assert.Equal(expectedResult, result);
        }


        [Theory] // bug fix
        [InlineData("", UserColumnsSorting.UserName, true, 0)]
        [InlineData("user1", UserColumnsSorting.Email, false, 1)]
        public async Task GetUsersPageAsync_ReturnsExpectedResults(
        string searchTerm, UserColumnsSorting column, bool asc, int pageNumber)
        {
            // Arrange
            var data = new GridModelDto<UserColumnsSorting>
            {
                Data = searchTerm,
                Column = column,
                Asc = asc,
                PageNumber = pageNumber,
            };

            var users = new List<User>
            {
                new() { Id = 1, UserName = "user1", Email = "user1@example.com" },
                new() { Id = 2, UserName = "user2", Email = "user2@example.com" },
            }.AsQueryable();

            _userManagerMock.Setup(m => m.Users).Returns(users);

            _queryBuilderMock
                .Setup(q => q.SearchQuery(It.IsAny<string>(), It.IsAny<string[]>()))
                .Returns((string searchValue, string[] columns) =>
                {
                    var parameter = Expression.Parameter(typeof(User), "x");
                    var property = Expression.Property(parameter, columns[0]);
                    var containsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) });
                    var containsCall = Expression.Call(property, containsMethod, Expression.Constant(searchValue));
                    var lambda = Expression.Lambda<Func<User, bool>>(containsCall, parameter);
                    return lambda;
                });
            _queryBuilderMock
                .Setup(q => q.OrderByQuery(It.IsAny<IQueryable<User>>(),
                    It.IsAny<string>(), It.IsAny<bool>()))
                .Returns((IQueryable<User> query, string column, bool ascOrder) =>
                {
                    var type = typeof(User);
                    var property = type.GetProperty(column);
                    var parameter = Expression.Parameter(type, "p");
                    var propertyAccess = Expression.MakeMemberAccess(parameter, property);
                    var orderByExpression = Expression.Lambda(propertyAccess, parameter);

                    var command = ascOrder ? "OrderBy" : "OrderByDescending";
                    var resultExpression = Expression.Call(typeof(Queryable), command, new Type[] { type, property.PropertyType },
                        query.Expression, Expression.Quote(orderByExpression));

                    return query.Provider.CreateQuery<User>(resultExpression);
                });

            // Act
            var result = await _userService.GetUsersPageAsync(data);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<GridModelResponse<UserDto>>(result);
            Assert.True(result.Items.Count() <= _pageSize);
        }
    }
}
