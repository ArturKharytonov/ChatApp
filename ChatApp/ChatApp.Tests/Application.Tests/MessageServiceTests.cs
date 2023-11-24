using System.Linq.Expressions;
using ChatApp.Application.Services.MessageService;
using ChatApp.Application.Services.QueryBuilder.Interfaces;
using ChatApp.Domain.DTOs.Http;
using ChatApp.Domain.DTOs.MessageDto;
using ChatApp.Domain.Rooms;
using ChatApp.Domain.Users;
using ChatApp.Tests.Fixtures.Services;
using Moq;
using Message = ChatApp.Domain.Messages.Message;

namespace ChatApp.Tests.Application.Tests
{
    public class MessageServiceTests : IClassFixture<MessageServiceFixture>
    {
        private readonly MessageServiceFixture _fixture;
        public MessageServiceTests()
        {
            _fixture = new MessageServiceFixture();
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

            _fixture.UnitOfWork
                .Setup(u => u.GetRepository<Room, int>())
                .Returns(_fixture.RoomRepositoryMock.Object);

            _fixture.SetupRoomRepository(room);

            // Act
            var result = await _fixture.MessageService.GetMessagesFromChat(roomId);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);

            var messageDto = result.First();
            Assert.Equal(messageId, messageDto.Id);
            Assert.Equal(senderUsername, messageDto.SenderUsername);
            Assert.Equal(messageContent, messageDto.Content);
        }

        [Theory]
        [InlineData(1, "Updated Content", true)]
        [InlineData(1, "Updated Content", false)]
        public async Task UpdateMessageAsync_MessageExists_ReturnsProperResult(int messageId, string updatedContent, bool success)
        {
            // Arrange
            var existingMessage = success
                ? new Message
                {
                    Id = messageId,
                    Content = "Original Content",
                }
                : null;

            _fixture.UnitOfWork.Setup(u => u.GetRepository<Message, int>())
                .Returns(_fixture.MessageRepositoryMock.Object);

            _fixture.MessageRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(existingMessage);

            var messageDto = new MessageDto
            {
                Id = messageId,
                Content = updatedContent
            };

            // Act
            var result = await _fixture.MessageService.UpdateMessageAsync(messageDto);

            // Assert
            if (success)
            {
                Assert.True(result);
                Assert.Equal(updatedContent, existingMessage.Content);
                _fixture.MessageRepositoryMock.Verify(r => r.Update(It.IsAny<Message>()), Times.Once);
                _fixture.UnitOfWork.Verify(u => u.SaveAsync(), Times.Once);
            }
            else
            {
                Assert.False(result);
                _fixture.MessageRepositoryMock.Verify(r => r.Update(It.IsAny<Message>()), Times.Never);
                _fixture.UnitOfWork.Verify(u => u.SaveAsync(), Times.Never);
            }
        }

        [Theory]
        [InlineData(1)]
        public async Task DeleteMessageAsync_DeletesMessage(int messageIdToDelete)
        {
            // Arrange
            _fixture.UnitOfWork.Setup(u => u.GetRepository<Message, int>())
                .Returns(_fixture.MessageRepositoryMock.Object);

            // Act
            await _fixture.MessageService.DeleteMessageAsync(messageIdToDelete);

            // Assert
            _fixture.MessageRepositoryMock.Verify(r => r.DeleteAsync(messageIdToDelete), Times.Once);
            _fixture.UnitOfWork.Verify(u => u.SaveAsync(), Times.Once);
        }

        [Theory]
        [InlineData("1", 1, "Test message", "TestUser", 1, "Test Room")]
        public async Task AddMessageAsync_ShouldAddMessage(string userId, int roomId, string content,
            string userName, int expectedUserId, string expectedRoomName)
        {
            // Arrange
            var addMessageDto =
                _fixture.SetupAddMessageTest(userId, roomId, content, userName, expectedUserId, expectedRoomName);

            // Act
            var result = await _fixture.MessageService.AddMessageAsync(addMessageDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(addMessageDto.Content, result.Content);
            Assert.Equal(addMessageDto.SentAt, result.SentAt);
            Assert.Equal(userName, result.SenderUsername);
            Assert.Equal(expectedRoomName, result.RoomName);

            _fixture.UnitOfWork.Verify(u => u.GetRepository<Room, int>(), Times.Once);
            _fixture.RoomRepositoryMock.Verify(r => r.GetByIdAsync(addMessageDto.RoomId), Times.Once);
            _fixture.UnitOfWork.Verify(u => u.GetRepository<Message, int>(), Times.Once);
            _fixture.MessageRepositoryMock.Verify(r => r.CreateAsync(It.IsAny<Message>()), Times.Once);
            _fixture.UnitOfWork.Verify(u => u.SaveAsync(), Times.Once);
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

    }
}
