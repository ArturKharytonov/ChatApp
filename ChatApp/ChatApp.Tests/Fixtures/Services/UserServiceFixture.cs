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

    }
}
