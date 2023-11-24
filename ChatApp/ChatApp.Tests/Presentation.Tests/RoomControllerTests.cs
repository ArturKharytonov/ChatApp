using ChatApp.Domain.DTOs.Http.Responses;
using ChatApp.Domain.DTOs.Http;
using ChatApp.Domain.DTOs.RoomDto;
using ChatApp.Domain.Enums;
using ChatApp.WebAPI.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ChatApp.Application.Services.RoomService.Interfaces;
using ChatApp.Application.Services.UserContext;
using ChatApp.Application.Services.UserContext.Interfaces;

namespace ChatApp.Tests.Presentation.Tests
{
    public class RoomControllerTests
    {
        private readonly Mock<IUserContext> _userContext;
        private readonly Mock<IRoomService> _roomService;
        private readonly RoomController _roomController;

        public RoomControllerTests()
        {
            _userContext = new Mock<IUserContext>();
            _roomService = new Mock<IRoomService>();
            _roomController = new RoomController(_roomService.Object, _userContext.Object);
        }

        [Theory]
        [InlineData("123", true)]
        [InlineData("", false)]
        public async Task GetRooms_ReturnsExpectedResult(string userIdString, bool expectOkResult)
        {
            // Arrange
            var model = new GridModelDto<RoomColumnsSorting>();
            var userId = string.IsNullOrEmpty(userIdString) ? 0 : int.Parse(userIdString);

            _userContext.Setup(context => context.GetUserId())
                        .Returns(userIdString);

            _roomService.Setup(service => service.GetRoomsPageAsync(userId, model))
                        .ReturnsAsync(new GridModelResponse<RoomDto>()); // maybe later create some more real model

            // Act
            var result = await _roomController.GetRooms(model);

            // Assert
            if (expectOkResult)
            {
                var okObjectResult = Assert.IsType<OkObjectResult>(result);
                var returnedRooms = Assert.IsAssignableFrom<GridModelResponse<RoomDto>>(okObjectResult.Value);

                Assert.NotNull(returnedRooms);
            }
            else
                Assert.IsType<BadRequestResult>(result);
            
            _userContext.Verify(context => context.GetUserId(), Times.Once);
            _roomService.Verify(service => service.GetRoomsPageAsync(userId, model), Times.Exactly(expectOkResult ? 1 : 0));
        }

        [Theory]
        [InlineData("Room1", "123", true, 1)] // Valid scenario
        [InlineData("Room2", "", false, null)] // Invalid scenario
        public async Task CreateRoom_ReturnsExpectedResult(string roomName, string userId, bool expectOkResult, int? expectedRoomId)
        {
            // Arrange
            _userContext.Setup(context => context.GetUserId())
                .Returns(userId);

            _roomService.Setup(service => service.CreateRoom(roomName, userId))
                .ReturnsAsync(expectedRoomId);


            // Act
            var result = await _roomController.CreateRoom(roomName);

            // Assert
            _userContext.Verify(context => context.GetUserId(), Times.Once);

            _roomService.Verify(service => service.CreateRoom(roomName, userId), Times.Exactly(expectOkResult ? 1 : 0));

            if (expectOkResult)
            {
                var okObjectResult = Assert.IsType<OkObjectResult>(result);
                var addRoomResponseDto = Assert.IsType<AddRoomResponseDto>(okObjectResult.Value);

                Assert.True(addRoomResponseDto.WasAdded);
                Assert.Equal(expectedRoomId, addRoomResponseDto.CreatedRoomId);
            }
            else
            {
                var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(result);
                var addRoomResponseDto = Assert.IsType<AddRoomResponseDto>(badRequestObjectResult.Value);

                Assert.False(addRoomResponseDto.WasAdded);
                Assert.Null(addRoomResponseDto.CreatedRoomId);
            }
        }

        [Theory]
        [InlineData("123", true)] // Valid scenario
        [InlineData("invalidId", false)] // Invalid scenario
        public async Task GetRoom_ReturnsExpectedResult(string id, bool expectOkResult)
        {
            // Arrange
            if (!int.TryParse(id, out var parsedId) && expectOkResult)
                return;
            
            _roomService.Setup(service => service.GetRoom(parsedId))!
                .ReturnsAsync(expectOkResult ? new RoomDto() : null);

            // Act
            var result = await _roomController.GetRoom(id);

            // Assert
            _roomService.Verify(service => service.GetRoom(parsedId), Times.Exactly(expectOkResult ? 1 : 0));

            if (expectOkResult)
            {
                var okObjectResult = Assert.IsType<OkObjectResult>(result);
                var returnedRoom = Assert.IsType<RoomDto>(okObjectResult.Value);

                Assert.NotNull(returnedRoom);
            }
            else
                Assert.IsType<BadRequestResult>(result);
            
        }
    }
}
