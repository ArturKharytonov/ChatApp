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
using ChatApp.Application.Services.QueryBuilder;
using ChatApp.Domain.Rooms;
using ChatApp.Application.Services.RoomService.Interfaces;
using ChatApp.Domain.DTOs.MessageDto;
using ChatApp.Domain.Enums;
using ChatApp.Persistence.UnitOfWork;
using Microsoft.EntityFrameworkCore;

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
        private readonly IQueryBuilder<RoomDto> _queryBuilder;

        public RoomServiceFixture()
        {
            UnitOfWorkMock = new Mock<IUnitOfWork>();
            QueryBuilderMock = new Mock<IQueryBuilder<RoomDto>>();
            UserManagerMock = new Mock<UserManager<User>>(
                Mock.Of<IUserStore<User>>(),
                null, null, null, null, null, null, null, null);
            RoomRepositoryMock = new Mock<IRepository<Room, int>>();
            RoomService = new RoomService(QueryBuilderMock.Object, UnitOfWorkMock.Object, UserManagerMock.Object);
            _queryBuilder = new QueryBuilder<RoomDto>();
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

        public void SetupRoomsPageService(IQueryable<Room> rooms)
        {
            UnitOfWorkMock.Setup(u => u.GetRepository<Room, int>())
                .Returns(RoomRepositoryMock.Object);

            var mockSet = new Mock<DbSet<Room>>();
            mockSet.As<IQueryable<Room>>().Setup(m => m.Provider).Returns(rooms.Provider);
            mockSet.As<IQueryable<Room>>().Setup(m => m.Expression).Returns(rooms.Expression);
            mockSet.As<IQueryable<Room>>().Setup(m => m.ElementType).Returns(rooms.ElementType);
            mockSet.As<IQueryable<Room>>().Setup(m => m.GetEnumerator()).Returns(rooms.GetEnumerator());

            RoomRepositoryMock
                .Setup(r => r.GetAllAsQueryableAsync())
                .ReturnsAsync(mockSet.Object);

            QueryBuilderMock
                .Setup(q => q.SearchQuery(It.IsAny<string>(), Enum.GetNames(typeof(RoomColumnsSorting))))
                .Returns((string searchValue, string[] columns) => _queryBuilder.SearchQuery(searchValue, columns));

            QueryBuilderMock
                .Setup(q => q.OrderByQuery(It.IsAny<IQueryable<RoomDto>>(), It.IsAny<string>(), It.IsAny<bool>()))
                .Returns((IQueryable<RoomDto> query, string sortColumn, bool ascOrder) => _queryBuilder.OrderByQuery(query, sortColumn, ascOrder));
        }
    }
}
