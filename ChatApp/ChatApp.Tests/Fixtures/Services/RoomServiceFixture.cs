using ChatApp.Application.Services.QueryBuilder.Interfaces;
using ChatApp.Application.Services.RoomService;
using ChatApp.Domain.DTOs.RoomDto;
using ChatApp.Domain.Users;
using ChatApp.Persistence.Common.Interfaces;
using ChatApp.Persistence.UnitOfWork.Interfaces;
using Microsoft.AspNetCore.Identity;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChatApp.Domain.Rooms;
using ChatApp.Application.Services.RoomService.Interfaces;

namespace ChatApp.Tests.Fixtures.Services
{
    public class RoomServiceFixture : IDisposable
    {
        public readonly Mock<IUnitOfWork> UnitOfWorkMock;
        public readonly Mock<IQueryBuilder<RoomDto>> QueryBuilderMock;
        public readonly Mock<UserManager<User>> UserManagerMock;
        public readonly Mock<IRepository<Room, int>> RoomRepositoryMock;
        public readonly RoomService RoomService;
        public readonly int PageSize = 5;

        public RoomServiceFixture()
        {
            UnitOfWorkMock = new Mock<IUnitOfWork>();
            QueryBuilderMock = new Mock<IQueryBuilder<RoomDto>>();
            UserManagerMock = new Mock<UserManager<User>>(
                Mock.Of<IUserStore<User>>(),
                null, null, null, null, null, null, null, null);
            RoomRepositoryMock = new Mock<IRepository<Room, int>>();
            RoomService = new RoomService(QueryBuilderMock.Object, UnitOfWorkMock.Object, UserManagerMock.Object);
        }
        public void Dispose() { }

        public void SetupCreateRoomTest(int creatorId, int expectedRoomId)
        {
            var user = new User { Id = creatorId, UserName = "TestUser" };

            UnitOfWorkMock.Setup(u => u.GetRepository<Room, int>())
                .Returns(RoomRepositoryMock.Object);

            RoomRepositoryMock.Setup(r => r.GetAllAsQueryableAsync())
                .ReturnsAsync(new List<Room>().AsQueryable());

            UserManagerMock.Setup(u => u.FindByIdAsync(creatorId.ToString()))
                .ReturnsAsync(user);

            RoomRepositoryMock.Setup(r => r.CreateAsync(It.IsAny<Room>()))
                .Callback<Room>(r => r.Id = expectedRoomId)
                .Returns(Task.CompletedTask);
        }
    }
}
