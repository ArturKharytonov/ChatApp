using ChatApp.Application.Services.MessageService;
using ChatApp.Application.Services.QueryBuilder;
using ChatApp.Application.Services.QueryBuilder.Interfaces;
using ChatApp.Domain.DTOs.MessageDto;
using ChatApp.Domain.Users;
using ChatApp.Persistence.Common.Interfaces;
using ChatApp.Persistence.UnitOfWork.Interfaces;
using Microsoft.AspNetCore.Identity;
using Moq;
using ChatApp.Domain.Messages;
using ChatApp.Domain.Rooms;
using ChatApp.Tests.Fixtures.Setups;
using ChatApp.Tests.Fixtures.Setups.Interfaces;
using ChatApp.Domain.DTOs.Http;
using ChatApp.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Tests.Fixtures.Services
{
    public class MessageServiceFixture : IDisposable
    {
        public Mock<IUnitOfWork> UnitOfWork { get; private set; }
        public Mock<IQueryBuilder<MessageDto>> QueryBuilderMock { get; private set; }
        public Mock<UserManager<User>> UserManagerMock { get; private set; }
        public Mock<IRepository<Message, int>> MessageRepositoryMock { get; private set; }
        public Mock<IRepository<Room, int>> RoomRepositoryMock { get; private set; }
        public readonly MessageService MessageService;
        public readonly int PageSize = 5;
        public readonly IRepositoryMockSetup<Room, int> RoomRepoMockSetup;

        private readonly IQueryBuilder<MessageDto> _queryBuilder;

        public MessageServiceFixture()
        {
            UnitOfWork = new Mock<IUnitOfWork>();
            QueryBuilderMock = new Mock<IQueryBuilder<MessageDto>>();
            UserManagerMock = new Mock<UserManager<User>>(
                Mock.Of<IUserStore<User>>(),
                null, null, null, null, null, null, null, null);

            MessageRepositoryMock = new Mock<IRepository<Message, int>>();
            RoomRepositoryMock = new Mock<IRepository<Room, int>>();

            MessageService = new MessageService(UnitOfWork.Object, QueryBuilderMock.Object, UserManagerMock.Object);
            RoomRepoMockSetup = new RepositoryMockSetup<Room, int>();
            _queryBuilder = new QueryBuilder<MessageDto>();

        }

        public void Dispose()
        {
            UnitOfWork?.Reset();
            QueryBuilderMock?.Reset();
            UserManagerMock?.Reset();
            MessageRepositoryMock?.Reset();
            RoomRepositoryMock?.Reset();
        }

        public void SetupRoomRepository(Room room)
        {
            RoomRepoMockSetup.SetupRepository(RoomRepositoryMock, room);
        }

        public AddMessageDto SetupAddMessageTest(string userId, int roomId, string content,
            string userName, int expectedUserId, string expectedRoomName)
        {
            var addMessageDto = new AddMessageDto
            {
                UserId = userId,
                RoomId = roomId,
                Content = content,
                SentAt = DateTime.UtcNow
            };

            var user = new User { Id = expectedUserId, UserName = userName };
            var room = new Room { Id = roomId, Name = expectedRoomName };

            UnitOfWork.Setup(u => u.GetRepository<Message, int>()).Returns(MessageRepositoryMock.Object);
            UnitOfWork.Setup(u => u.GetRepository<Room, int>()).Returns(RoomRepositoryMock.Object);

            UserManagerMock.Setup(u => u.FindByIdAsync(addMessageDto.UserId))
                .ReturnsAsync(user);

            RoomRepositoryMock.Setup(r => r.GetByIdAsync(addMessageDto.RoomId))
                .ReturnsAsync(room);

            return addMessageDto;
        }

        public void SetupMessagesPageService(IQueryable<Message> messages)
        {
            UnitOfWork.Setup(u => u.GetRepository<Message, int>())
                .Returns(MessageRepositoryMock.Object);
            UnitOfWork.Setup(u => u.GetRepository<Room, int>())
                .Returns(RoomRepositoryMock.Object);

            var mockSet = new Mock<DbSet<Message>>();
            mockSet.As<IQueryable<Message>>().Setup(m => m.Provider).Returns(messages.Provider);
            mockSet.As<IQueryable<Message>>().Setup(m => m.Expression).Returns(messages.Expression);
            mockSet.As<IQueryable<Message>>().Setup(m => m.ElementType).Returns(messages.ElementType);
            mockSet.As<IQueryable<Message>>().Setup(m => m.GetEnumerator()).Returns(messages.GetEnumerator());

            MessageRepositoryMock
                .Setup(r => r.GetAllAsQueryableAsync())
                .ReturnsAsync(mockSet.Object);

            UserManagerMock
                .Setup(u => u.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync((string id) => new User { Id = int.Parse(id), UserName = $"User{id}" });

            RoomRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((int id) => new Room { Id = id, Name = $"Room{id}" });

            QueryBuilderMock
                .Setup(q => q.SearchQuery(It.IsAny<string>(), Enum.GetNames(typeof(MessageColumnsSorting))))
                .Returns((string searchValue, string[] columns) => _queryBuilder.SearchQuery(searchValue, columns));

            QueryBuilderMock
                .Setup(q => q.OrderByQuery(It.IsAny<IQueryable<MessageDto>>(), It.IsAny<string>(), It.IsAny<bool>()))
                .Returns((IQueryable<MessageDto> query, string sortColumn, bool ascOrder) => _queryBuilder.OrderByQuery(query, sortColumn, ascOrder));
        }
    }
}
