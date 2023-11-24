using ChatApp.Domain.Users;
using ChatApp.Persistence.Common.Interfaces;
using Microsoft.AspNetCore.Identity;
using Moq;
using System.Linq.Expressions;
using ChatApp.Domain.DTOs.Http;
using ChatApp.Domain.Rooms;
using ChatApp.Domain.DTOs.UserDto;
using ChatApp.Domain.Enums;
using ChatApp.Domain.DTOs.Http.Responses;
using ChatApp.Tests.Fixtures.Services;
using Radzen;
using System.Data.Entity.Core.Common.CommandTrees;
using System.Xml;
using ChatApp.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using System;
using MockQueryable.Moq;

namespace ChatApp.Tests.Application.Tests
{
    public class UserServiceTests : IClassFixture<UserServiceFixture>
    {

        private readonly UserServiceFixture _fixture;
        public UserServiceTests()
        {
            _fixture = new UserServiceFixture();
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
            };

            var mock = users.BuildMock();

            _fixture.UserManagerMock.Setup(m => m.Users).Returns(mock);
            _fixture.QueryBuilderMock
                .Setup(q => q.SearchQuery(It.IsAny<string>(), Enum.GetNames(data.Column.GetType())))
                .Returns((string searchValue, string[] columns) =>
                {
                    var parameter = Expression.Parameter(typeof(User), "x");

                    Expression? expression = null;

                    foreach (var name in columns)
                    {
                        var containsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) });

                        var property = Expression.Property(parameter, name);
                        var propertyAsObject = Expression.Convert(property, typeof(object));
                        var nullCheck = Expression.ReferenceEqual(propertyAsObject, Expression.Constant(null));

                        Expression stringify = property.Type == typeof(string)
                            ? property
                            : Expression.Call(property, property.Type.GetMethod("ToString", Type.EmptyTypes));

                        var containsCall = Expression.Call(stringify, containsMethod, Expression.Constant(searchValue));
                        var conditionalExpression = Expression.Condition(nullCheck, Expression.Constant(false), containsCall);

                        if (expression == null)
                            expression = conditionalExpression;
                        else
                            expression = Expression.OrElse(expression, conditionalExpression);
                    }
                    var lambda = Expression.Lambda<Func<User, bool>>(expression!, parameter);
                    return lambda;
                });

            _fixture.QueryBuilderMock
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
            var result = await _fixture.UserService.GetUsersPageAsync(data);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<GridModelResponse<UserDto>>(result);
            Assert.True(result.Items.Count() <= _fixture.PageSize);
        }
    }
}

