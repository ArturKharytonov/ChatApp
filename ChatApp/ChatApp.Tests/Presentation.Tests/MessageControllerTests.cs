using ChatApp.Application.Services.MessageService.Interfaces;
using ChatApp.Domain.DTOs.Http;
using ChatApp.Domain.DTOs.MessageDto;
using ChatApp.Domain.Enums;
using ChatApp.WebAPI.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChatApp.Domain.DTOs.Http.Responses;

namespace ChatApp.Tests.Presentation.Tests
{
    public class MessageControllerTests
    {
        private readonly Mock<IMessageService> _messageService;
        private readonly MessageController _messageController;
        public MessageControllerTests()
        {
            _messageService = new Mock<IMessageService>();
            _messageController = new MessageController(_messageService.Object);
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

            _messageService
                .Setup(service => service.GetMessagePageAsync(It.IsAny<GridModelDto<MessageColumnsSorting>>()))
                .ReturnsAsync(mockedData);

            // Act
            var result = await _messageController.GetPageAsync(new GridModelDto<MessageColumnsSorting>());

            // Assert
            var okObjectResult = Assert.IsType<OkObjectResult>(result);
            var returnedData = Assert.IsType<GridModelResponse<MessageDto>>(okObjectResult.Value);

            Assert.NotNull(returnedData);
            Assert.NotNull(returnedData.Items);
            Assert.Equal(0, returnedData.TotalCount);

            _messageService.Verify(service => service.GetMessagePageAsync(It.IsAny<GridModelDto<MessageColumnsSorting>>()), Times.Once);
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

            _messageService
                .Setup(service => service.AddMessageAsync(It.IsAny<AddMessageDto>()))
                .ReturnsAsync(mockedMessageDto);


            // Act
            var result = await _messageController.AddMessageAsync(addMessageDto);

            // Assert
            var okObjectResult = Assert.IsType<OkObjectResult>(result);
            var returnedMessageDto = Assert.IsType<MessageDto>(okObjectResult.Value);

            Assert.NotNull(returnedMessageDto);
            Assert.Equal(returnedMessageDto.Content, addMessageDto.Content);

            _messageService.Verify(service => service.AddMessageAsync(It.IsAny<AddMessageDto>()), Times.Once);
        }


        [Theory]
        [InlineData("123")]
        public async Task GetAllMessagesAsync_ReturnsOkResult(string roomId)
        {
            // Arrange
            var mockedMessages = new List<MessageDto>
            {
                new MessageDto
                {
                    Id = 1,
                    Content = "Hello, World!",
                    RoomName = "test1",
                    SenderUsername = "test1",
                    SentAt = DateTime.UtcNow.AddMinutes(-10)
                },
                new MessageDto
                {
                    Id = 2,
                    Content = "How are you?",
                    RoomName = "test2",
                    SenderUsername = "test2",
                    SentAt = DateTime.UtcNow.AddMinutes(-5)
                }
            };

            _messageService
                .Setup(service => service.GetMessagesFromChat(roomId))
                .ReturnsAsync(mockedMessages);

            // Act
            var result = await _messageController.GetAllMessagesAsync(roomId);

            // Assert
            var okObjectResult = Assert.IsType<OkObjectResult>(result);
            var returnedMessages = Assert.IsAssignableFrom<IEnumerable<MessageDto>>(okObjectResult.Value);

            Assert.NotNull(returnedMessages);

            _messageService.Verify(service => service.GetMessagesFromChat(roomId), Times.Once);
        }

        [Theory]
        [InlineData(true, "Updated successfully")]
        [InlineData(false, "Smth went wrong")]
        public async Task UpdateMessageAsync_ReturnsExpectedResult(bool updateSuccess, string expectedMessage)
        {
            // Arrange
            _messageService
                .Setup(service => service.UpdateMessageAsync(It.IsAny<MessageDto>()))
                .ReturnsAsync(updateSuccess);

            // Act
            var result = await _messageController.UpdateMessageAsync(new MessageDto());

            // Assert
            _messageService.Verify(service => service.UpdateMessageAsync(It.IsAny<MessageDto>()), Times.Once);

            if (updateSuccess)
            {
                var okObjectResult = Assert.IsType<OkObjectResult>(result);
                var updateResponseDto = Assert.IsType<UpdateMessageResponseDto>(okObjectResult.Value);

                // Additional assertions based on the expected data or behavior
                Assert.True(updateResponseDto.Successful);
                Assert.Equal(expectedMessage, updateResponseDto.Message);
            }
            else
            {
                var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(result);
                var updateResponseDto = Assert.IsType<UpdateMessageResponseDto>(badRequestObjectResult.Value);

                // Additional assertions based on the expected data or behavior
                Assert.False(updateResponseDto.Successful);
                Assert.Equal(expectedMessage, updateResponseDto.Message);
            }
        }
    }
}
