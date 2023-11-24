using ChatApp.Application.Services.MessageService;
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

namespace ChatApp.Tests.Fixtures.Services
{
    internal class MessageServiceFixture : IDisposable
    {
        public readonly Mock<IUnitOfWork> UnitOfWork;
        public readonly Mock<IQueryBuilder<MessageDto>> QueryBuilderMock;
        public readonly Mock<UserManager<User>> UserManagerMock;
        public readonly Mock<IRepository<Message, int>> MessageRepositoryMock;
        public readonly Mock<IRepository<Room, int>> RoomRepositoryMock;
        public readonly MessageService MessageService;

        public readonly IRepositoryMockSetup<Room, int> RoomRepoMockSetup;
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
        }
        public void Dispose() { }

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

            UserManagerMock.Setup(u => u.FindByIdAsync(addMessageDto.UserId))
                .ReturnsAsync(user);

            UnitOfWork.Setup(u => u.GetRepository<Room, int>())
                .Returns(RoomRepositoryMock.Object);

            RoomRepositoryMock.Setup(r => r.GetByIdAsync(addMessageDto.RoomId))
                .ReturnsAsync(room);

            UnitOfWork.Setup(u => u.GetRepository<Message, int>())
                .Returns(MessageRepositoryMock.Object);

            return addMessageDto;
        }
    }
}
