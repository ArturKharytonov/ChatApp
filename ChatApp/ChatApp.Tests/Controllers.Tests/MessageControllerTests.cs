using ChatApp.Domain.DTOs.Http;
using ChatApp.Domain.DTOs.Http.Responses;
using ChatApp.Domain.DTOs.MessageDto;
using ChatApp.Domain.Enums;
using ChatApp.Tests.Fixtures.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace ChatApp.Tests.Controllers.Tests
{
    public class MessageControllerTests : IClassFixture<MessageControllerFixture>
    {
        private readonly MessageControllerFixture _fixture;
        public MessageControllerTests(MessageControllerFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public async Task GetPageAsync_ReturnsOkResult()
        {
            // Arrange
            var mockedData = new GridModelResponse<MessageDto>
            {
                Items = new List<MessageDto>(),
                TotalCount = 0
            };

            _fixture.MessageService
                .Setup(service => service.GetMessagePageAsync(It.IsAny<GridModelDto<MessageColumnsSorting>>()))
                .ReturnsAsync(mockedData);

            // Act
            var result = await _fixture.MessageController.GetPageAsync(new GridModelDto<MessageColumnsSorting>());

            // Assert
            var okObjectResult = Assert.IsType<OkObjectResult>(result);
            var returnedData = Assert.IsType<GridModelResponse<MessageDto>>(okObjectResult.Value);

            Assert.NotNull(returnedData);
            Assert.NotNull(returnedData.Items);
            Assert.Equal(0, returnedData.TotalCount);

            _fixture.MessageService.Verify(service => service.GetMessagePageAsync(It.IsAny<GridModelDto<MessageColumnsSorting>>()),
                Times.Once);

            _fixture.Dispose();
        }

        [Fact]
        public async Task AddMessageAsync_ReturnsOkResult()
        {
            // Arrange
            var addMessageDto = new AddMessageDto
            {
                Content = "Test content",
                RoomId = 123,
                UserId = "user123",
                SentAt = DateTime.UtcNow
            };

            var mockedMessageDto = new MessageDto
            {
                Content = addMessageDto.Content,
                SentAt = addMessageDto.SentAt
            };

            _fixture.MessageService
                .Setup(service => service.AddMessageAsync(It.IsAny<AddMessageDto>()))
                .ReturnsAsync(mockedMessageDto);


            // Act
            var result = await _fixture.MessageController.AddMessageAsync(addMessageDto);

            // Assert
            var okObjectResult = Assert.IsType<OkObjectResult>(result);
            var returnedMessageDto = Assert.IsType<MessageDto>(okObjectResult.Value);

            Assert.NotNull(returnedMessageDto);
            Assert.Equal(returnedMessageDto.Content, addMessageDto.Content);

            _fixture.MessageService.Verify(service => service.AddMessageAsync(It.IsAny<AddMessageDto>()), Times.Once);
            _fixture.Dispose();
        }


        [Theory]
        [InlineData("123")]
        public async Task GetAllMessagesAsync_ReturnsOkResult(string roomId)
        {
            // Arrange
            var mockedMessages = new List<MessageDto>
            {
                new()
                {
                    Id = 1,
                    Content = "Hello, World!",
                    RoomName = "test1",
                    SenderUsername = "test1",
                    SentAt = DateTime.UtcNow.AddMinutes(-10)
                },
                new()
                {
                    Id = 2,
                    Content = "How are you?",
                    RoomName = "test2",
                    SenderUsername = "test2",
                    SentAt = DateTime.UtcNow.AddMinutes(-5)
                }
            };

            _fixture.MessageService
                .Setup(service => service.GetMessagesFromChat(roomId))
                .ReturnsAsync(mockedMessages);

            // Act
            var result = await _fixture.MessageController.GetAllMessagesAsync(roomId);

            // Assert
            var okObjectResult = Assert.IsType<OkObjectResult>(result);
            var returnedMessages = Assert.IsAssignableFrom<IEnumerable<MessageDto>>(okObjectResult.Value);

            Assert.NotNull(returnedMessages);

            _fixture.MessageService.Verify(service => service.GetMessagesFromChat(roomId), Times.Once);
            _fixture.Dispose();
        }

        [Theory]
        [InlineData(true, "Updated successfully")]
        [InlineData(false, "Smth went wrong")]
        public async Task UpdateMessageAsync_ReturnsExpectedResult(bool updateSuccess, string expectedMessage)
        {
            // Arrange
            _fixture.MessageService
                .Setup(service => service.UpdateMessageAsync(It.IsAny<MessageDto>()))
                .ReturnsAsync(updateSuccess);

            // Act
            var result = await _fixture.MessageController.UpdateMessageAsync(new MessageDto());

            // Assert
            _fixture.MessageService.Verify(service => service.UpdateMessageAsync(It.IsAny<MessageDto>()), Times.Once);

            if (updateSuccess)
            {
                var okObjectResult = Assert.IsType<OkObjectResult>(result);
                var updateResponseDto = Assert.IsType<UpdateMessageResponseDto>(okObjectResult.Value);

                Assert.True(updateResponseDto.Successful);
                Assert.Equal(expectedMessage, updateResponseDto.Message);
            }
            else
            {
                var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(result);
                var updateResponseDto = Assert.IsType<UpdateMessageResponseDto>(badRequestObjectResult.Value);

                Assert.False(updateResponseDto.Successful);
                Assert.Equal(expectedMessage, updateResponseDto.Message);
            }
            _fixture.Dispose();
        }
    }
}
