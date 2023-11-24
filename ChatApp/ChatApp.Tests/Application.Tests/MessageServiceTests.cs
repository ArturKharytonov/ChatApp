using System.Linq.Expressions;
using ChatApp.Application.Services.MessageService;
using ChatApp.Application.Services.QueryBuilder.Interfaces;
using ChatApp.Domain.DTOs.Http;
using ChatApp.Domain.DTOs.MessageDto;
using ChatApp.Domain.Enums;
using ChatApp.Domain.Rooms;
using ChatApp.Domain.Users;
using ChatApp.Persistence.Common.Interfaces;
using ChatApp.Persistence.UnitOfWork.Interfaces;
using ChatApp.UI.Pages.User;
using Microsoft.AspNetCore.Identity;
using Moq;
using Message = ChatApp.Domain.Messages.Message;

namespace ChatApp.Tests.Application.Tests
{
    public class MessageServiceTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWork;
        private readonly Mock<IQueryBuilder<MessageDto>> _queryBuilderMock;
        private readonly Mock<UserManager<User>> _userManagerMock;
        private readonly Mock<IRepository<Message, int>> _messageRepositoryMock;
        private readonly Mock<IRepository<Room, int>> _roomRepositoryMock;
        private readonly MessageService _messageService;

        public MessageServiceTests()
        {
            _unitOfWork = new Mock<IUnitOfWork>();
            _queryBuilderMock = new Mock<IQueryBuilder<MessageDto>>();
            _userManagerMock = new Mock<UserManager<User>>(
                Mock.Of<IUserStore<User>>(),
                null, null, null, null, null, null, null, null);
            _messageRepositoryMock = new Mock<IRepository<Message, int>>();
            _roomRepositoryMock = new Mock<IRepository<Room, int>>();
            _messageService = new MessageService(_unitOfWork.Object, _queryBuilderMock.Object, _userManagerMock.Object);
        }


        [Theory]
        [InlineData("1", 1, "SenderUser", "Test message")]
        public async Task GetMessagesFromChat_ReturnsMessages(string roomId, int messageId, 
            string senderUsername, string messageContent)
        {
            var room = new Room
            {
                Id = 1,
                Messages = new List<Message>
                {
                    new()
                    {
                        Id = messageId,
                        Sender = new User { UserName = senderUsername },
                        Content = messageContent,
                        SentAt = DateTime.Now,
                        Room = new Room { Id = 1, Name = "Test Room" }
                    }
                }
            };
            _unitOfWork
                .Setup(u => u.GetRepository<Room, int>())
                .Returns(_roomRepositoryMock.Object);

            _roomRepositoryMock
                .Setup(r => r.GetByIdAsync(It.IsAny<int>(),
                    It.IsAny<Expression<Func<Room, object>>[]>()))
                .ReturnsAsync(room);

            var chatService = new MessageService(_unitOfWork.Object, _queryBuilderMock.Object, null);

            // Act
            var result = await chatService.GetMessagesFromChat(roomId);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);

            var messageDto = result.First();
            Assert.Equal(messageId, messageDto.Id);
            Assert.Equal(senderUsername, messageDto.SenderUsername);
            Assert.Equal(messageContent, messageDto.Content);
        }

        [Fact] // bug: fix
        public async Task GetMessagePageAsync_ShouldReturnCorrectGridModelResponse()
        {
            //// Arrange
            //_queryBuilderMock.Setup(qb => qb.SearchQuery(It.IsAny<string>(), It.IsAny<string[]>()))
            //               .Returns<Expression<Func<MessageDto, bool>>>(expr => m => true);

            //_queryBuilderMock.Setup(qb => qb.OrderByQuery(It.IsAny<IQueryable<MessageDto>>(), It.IsAny<string>(), It.IsAny<bool>()))
            //               .Returns<IQueryable<MessageDto>, string, bool>((query, column, asc) => query); // Mock order by query to return the same query for testing

            //var data = new GridModelDto<MessageColumnsSorting>
            //{
            //    PageNumber = 1,
            //    Data = "yourSearchTerm",
            //    Column = MessageColumnsSorting.RoomName, 
            //    Asc = true,
            //    Sorting = true
            //};

            //var roomList = new List<Room>
            //{
            //    new Room { Id = 1, Name = "Room1" },
            //    new Room { Id = 2, Name = "Room2" },
            //};

            //var messageList = new List<Message>
            //{
            //    new Message { Id = 1, Content = "Message1", SentAt = DateTime.Now, SenderId = 1, RoomId = 1 },
            //    new Message { Id = 2, Content = "Message2", SentAt = DateTime.Now, SenderId = 2, RoomId = 2 },
            //};


            //_messageRepositoryMock.Setup(m => m.GetAllAsQueryableAsync())
            //    .ReturnsAsync(messageList.AsQueryable());

            //_userManagerMock.Setup(um => um.FindByIdAsync(It.IsAny<string>()))
            //               .ReturnsAsync(new User() { UserName = "testuser" }); // Replace with an actual user instance

            //_roomRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<int>()))
            //    .ReturnsAsync((int id) => roomList.FirstOrDefault(r => r.Id == id));

            //// Act
            //var result = await _messageService.GetMessagePageAsync(data);

            //// Assert
            //Assert.NotNull(result);
            //Assert.NotNull(result.Items);
            //Assert.Equal(1, result.Items.Count());
            //Assert.Equal(messageList.Count, result.TotalCount);

            //_queryBuilderMock.Verify(qb =>
            //    qb.SearchQuery("yourSearchTerm", It.IsAny<string[]>()), Times.Once);
            //_queryBuilderMock.Verify(qb =>
            //    qb.OrderByQuery(It.IsAny<IQueryable<MessageDto>>(), "RoomName", true), Times.Once);
        }

        [Theory]
        [InlineData(1, "Updated Content")]
        public async Task UpdateMessageAsync_MessageExists_ReturnsTrue(int messageId, string updatedContent)
        {
            // Arrange
            var existingMessage = new Message
            {
                Id = messageId,
                Content = "Original Content",
            };

            _unitOfWork.Setup(u => u.GetRepository<Message, int>())
                .Returns(_messageRepositoryMock.Object);

            _messageRepositoryMock.Setup(r => r.GetByIdAsync(messageId))
                .ReturnsAsync(existingMessage);

            var messageDto = new MessageDto
            {
                Id = messageId,
                Content = updatedContent
            };

            // Act
            var result = await _messageService.UpdateMessageAsync(messageDto);

            // Assert
            Assert.True(result);
            Assert.Equal(updatedContent, existingMessage.Content);
            _messageRepositoryMock.Verify(r => r.Update(It.IsAny<Message>()), Times.Once);
            _unitOfWork.Verify(u => u.SaveAsync(), Times.Once);
        }

        [Theory]
        [InlineData(1, "Updated Content")]
        public async Task UpdateMessageAsync_MessageNotFound_ReturnsFalse(int messageId, string updatedContent)
        {
            _unitOfWork.Setup(u => u.GetRepository<Message, int>())
                .Returns(_messageRepositoryMock.Object);
            _messageRepositoryMock.Setup(r => r.GetByIdAsync(messageId))
                .ReturnsAsync((Message)null);

            var messageDto = new MessageDto
            {
                Id = messageId,
                Content = updatedContent
            };

            // Act
            var result = await _messageService.UpdateMessageAsync(messageDto);

            // Assert
            Assert.False(result);
            _messageRepositoryMock.Verify(r => r.Update(It.IsAny<Message>()), Times.Never);
            _unitOfWork.Verify(u => u.SaveAsync(), Times.Never);
        }

        [Theory]
        [InlineData(1)]
        public async Task DeleteMessageAsync_DeletesMessage(int messageIdToDelete)
        {
            // Arrange
            _unitOfWork.Setup(u => u.GetRepository<Message, int>())
                .Returns(_messageRepositoryMock.Object);

            // Act
            await _messageService.DeleteMessageAsync(messageIdToDelete);

            // Assert
            _messageRepositoryMock.Verify(r => r.DeleteAsync(messageIdToDelete), Times.Once);
            _unitOfWork.Verify(u => u.SaveAsync(), Times.Once);
        }

        [Theory]
        [InlineData("1", 1, "Test message", "TestUser", 1, "Test Room")]
        public async Task AddMessageAsync_ShouldAddMessage(string userId, int roomId, string content,
            string userName, int expectedUserId, string expectedRoomName)
        {
            // Arrange
            var addMessageDto = new AddMessageDto
            {
                UserId = userId,
                RoomId = roomId,
                Content = content,
                SentAt = DateTime.UtcNow
            };

            var user = new User { Id = expectedUserId, UserName = userName };
            var room = new Room { Id = roomId, Name = expectedRoomName };


            _userManagerMock.Setup(u => u.FindByIdAsync(addMessageDto.UserId))
                .ReturnsAsync(user);

            _unitOfWork.Setup(u => u.GetRepository<Room, int>())
                .Returns(_roomRepositoryMock.Object);

            _roomRepositoryMock.Setup(r => r.GetByIdAsync(addMessageDto.RoomId))
                .ReturnsAsync(room);

            _unitOfWork.Setup(u => u.GetRepository<Message, int>())
                .Returns(_messageRepositoryMock.Object);

            // Act
            var result = await _messageService.AddMessageAsync(addMessageDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(addMessageDto.Content, result.Content);
            Assert.Equal(addMessageDto.SentAt, result.SentAt);
            Assert.Equal(user.UserName, result.SenderUsername);
            Assert.Equal(room.Name, result.RoomName);

            _unitOfWork.Verify(u => u.GetRepository<Room, int>(), Times.Once);
            _roomRepositoryMock.Verify(r => r.GetByIdAsync(addMessageDto.RoomId), Times.Once);
            _unitOfWork.Verify(u => u.GetRepository<Message, int>(), Times.Once);
            _messageRepositoryMock.Verify(r => r.CreateAsync(It.IsAny<Message>()), Times.Once);
            _unitOfWork.Verify(u => u.SaveAsync(), Times.Once);
        }
    }
}
