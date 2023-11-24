using ChatApp.Persistence.UnitOfWork.Interfaces;
using Moq;
using ChatApp.Domain.Users;
using ChatApp.Persistence.Common.Interfaces;
using ChatApp.Application.Services.QueryBuilder.Interfaces;
using ChatApp.Application.Services.UserService;
using Microsoft.AspNetCore.Identity;
using System.Linq.Expressions;
using ChatApp.Domain.Rooms;
using ChatApp.Tests.Fixtures.Setups;
using ChatApp.Tests.Fixtures.Setups.Interfaces;
using ChatApp.Domain.DTOs.Http;
using ChatApp.Application.Services.QueryBuilder;
using ChatApp.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Tests.Fixtures.Services
{
    public class UserServiceFixture : IDisposable
    {
        public readonly Mock<UserManager<User>> UserManagerMock;
        public readonly Mock<IQueryBuilder<User>> QueryBuilderMock;
        public readonly Mock<IUnitOfWork> UnitOfWorkMock;
        public readonly Mock<IRepository<User, int>> UserRepository;
        public readonly Mock<IRepository<Room, int>> RoomRepository;
        public readonly UserService UserService;
        public readonly int PageSize = 5;

        public readonly IRepositoryMockSetup<User, int> UserRepoMockSetup;
        public readonly IRepositoryMockSetup<Room, int> RoomRepoMockSetup;

        private readonly IQueryBuilder<User> _queryBuilder;
        public UserServiceFixture()
        {
            UserManagerMock = new Mock<UserManager<User>>(
                Mock.Of<IUserStore<User>>(),
                null, null, null, null, null, null, null, null);
            QueryBuilderMock = new Mock<IQueryBuilder<User>>();
            UnitOfWorkMock = new Mock<IUnitOfWork>();
            UserRepository = new Mock<IRepository<User, int>>();
            RoomRepository = new Mock<IRepository<Room, int>>();
            UserService = new UserService(UserManagerMock.Object, QueryBuilderMock.Object, UnitOfWorkMock.Object);

            RoomRepoMockSetup = new RepositoryMockSetup<Room, int>();
            UserRepoMockSetup = new RepositoryMockSetup<User, int>();
            _queryBuilder = new QueryBuilder<User>();

        }

        public void Dispose() { }

        public void SetupUserRepository(User user)
        {
            UserRepoMockSetup.SetupRepository(UserRepository, user);
        }
        public void SetupRoomRepository(Room room)
        {
            RoomRepoMockSetup.SetupRepository(RoomRepository, room);
        }
        public void SetupChangePasswordTest(bool userExists, bool passwordMatches)
        {
            var userId = "1";
            var user = userExists ? new User { Id = 1 } : null;

            UserManagerMock
                .Setup(u => u.FindByIdAsync(userId))
                .ReturnsAsync(user);

            UserManagerMock
                .Setup(u => u.CheckPasswordAsync(user, "currentPassword"))
                .ReturnsAsync(passwordMatches);

            UserManagerMock
                .Setup(u => u.GeneratePasswordResetTokenAsync(user))
                .ReturnsAsync("token");

            UserManagerMock
                .Setup(u => u.ResetPasswordAsync(user, "token", "newPassword"))
                .ReturnsAsync(IdentityResult.Success);
        }
        public void SetupUpdateUserTest(bool userExists, bool doesUsernameExist)
        {
            var userId = "1";
            var existingUser = userExists ? new User { Id = 1, UserName = "existingUsername" } : null;

            UserManagerMock
                .Setup(u => u.FindByIdAsync(userId))
                .ReturnsAsync(existingUser);

            UserManagerMock
                .Setup(u => u.FindByNameAsync(It.IsAny<string>()))
                .ReturnsAsync(doesUsernameExist ? new User() : null);

            UserManagerMock
                .Setup(u => u.UpdateAsync(existingUser))
                .ReturnsAsync(IdentityResult.Success);
        }

        public void SetupUsersPageService(IQueryable<User> users)
        {
            var mockSet = new Mock<DbSet<User>>();
            mockSet.As<IQueryable<User>>().Setup(m => m.Provider).Returns(users.Provider);
            mockSet.As<IQueryable<User>>().Setup(m => m.Expression).Returns(users.Expression);
            mockSet.As<IQueryable<User>>().Setup(m => m.ElementType).Returns(users.ElementType);
            mockSet.As<IQueryable<User>>().Setup(m => m.GetEnumerator()).Returns(users.GetEnumerator());

            UserManagerMock.Setup(u => u.Users).Returns(mockSet.Object);

            QueryBuilderMock
                .Setup(q => q.SearchQuery(It.IsAny<string>(), Enum.GetNames(typeof(UserColumnsSorting))))
                .Returns((string searchValue, string[] columns) => _queryBuilder.SearchQuery(searchValue, columns));

            QueryBuilderMock
                .Setup(q => q.OrderByQuery(It.IsAny<IQueryable<User>>(), It.IsAny<string>(), It.IsAny<bool>()))
                .Returns((IQueryable<User> query, string sortColumn, bool ascOrder) => _queryBuilder.OrderByQuery(query, sortColumn, ascOrder));
        }
    }
}
