using ChatApp.Domain.DTOs.Http;
using ChatApp.Domain.DTOs.Http.Responses;
using ChatApp.Domain.DTOs.RoomDto;
using ChatApp.Domain.Enums;
using ChatApp.Tests.Fixtures.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace ChatApp.Tests.Controllers.Tests
{
    public class RoomControllerTests : IClassFixture<RoomControllerFixture>
    {
        private readonly RoomControllerFixture _fixture;

        public RoomControllerTests(RoomControllerFixture fixture)
        {
            _fixture = fixture;
        }

        [Theory]
        [InlineData("123", true)]
        [InlineData("", false)]
        public async Task GetRooms_ReturnsExpectedResult(string userIdString, bool expectOkResult)
        {
            // Arrange
            var model = new GridModelDto<RoomColumnsSorting>();
            var userId = string.IsNullOrEmpty(userIdString) ? 0 : int.Parse(userIdString);

            _fixture.UserContext.Setup(context => context.GetUserId())
                        .Returns(userIdString);

            _fixture.RoomService.Setup(service => service.GetRoomsPageAsync(userId, model))
                        .ReturnsAsync(new GridModelResponse<RoomDto>()); // maybe later create some more real model

            // Act
            var result = await _fixture.RoomController.GetRooms(model);

            // Assert
            if (expectOkResult)
            {
                var okObjectResult = Assert.IsType<OkObjectResult>(result);
                var returnedRooms = Assert.IsAssignableFrom<GridModelResponse<RoomDto>>(okObjectResult.Value);

                Assert.NotNull(returnedRooms);
            }
            else
                Assert.IsType<BadRequestResult>(result);

            _fixture.UserContext.Verify(context => context.GetUserId(), Times.Once);
            _fixture.RoomService.Verify(service => service.GetRoomsPageAsync(userId, model), Times.Exactly(expectOkResult ? 1 : 0));

            _fixture.Dispose();
        }

        [Theory]
        [InlineData("Room1", "123", true, 1)] // Valid scenario
        [InlineData("Room2", "", false, null)] // Invalid scenario
        public async Task CreateRoom_ReturnsExpectedResult(string roomName, string userId, bool expectOkResult, int? expectedRoomId)
        {
            // Arrange
            _fixture.UserContext.Setup(context => context.GetUserId())
                .Returns(userId);

            _fixture.RoomService.Setup(service => service.CreateRoom(roomName, userId))
                .ReturnsAsync(expectedRoomId);


            // Act
            var result = await _fixture.RoomController.CreateRoom(roomName);

            // Assert
            _fixture.UserContext.Verify(context => context.GetUserId(), Times.Once);

            _fixture.RoomService.Verify(service => service.CreateRoom(roomName, userId), Times.Exactly(expectOkResult ? 1 : 0));

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
            _fixture.Dispose();
        }

        [Theory]
        [InlineData("123", true)] // Valid scenario
        [InlineData("invalidId", false)] // Invalid scenario
        public async Task GetRoom_ReturnsExpectedResult(string id, bool expectOkResult)
        {
            // Arrange
            if (!int.TryParse(id, out var parsedId) && expectOkResult)
                return;

            _fixture.RoomService.Setup(service => service.GetRoom(parsedId))!
                .ReturnsAsync(expectOkResult ? new RoomDto() : null);

            // Act
            var result = await _fixture.RoomController.GetRoom(id);

            // Assert
            _fixture.RoomService.Verify(service => service.GetRoom(parsedId), Times.Exactly(expectOkResult ? 1 : 0));

            if (expectOkResult)
            {
                var okObjectResult = Assert.IsType<OkObjectResult>(result);
                var returnedRoom = Assert.IsType<RoomDto>(okObjectResult.Value);

                Assert.NotNull(returnedRoom);
            }
            else
                Assert.IsType<BadRequestResult>(result);

            _fixture.Dispose();
        }
    }
}
