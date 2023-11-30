using ChatApp.Domain.DTOs.Http.Responses;
using ChatApp.Domain.DTOs.Http;
using ChatApp.Domain.DTOs.RoomDto;
using ChatApp.Domain.Enums;
using ChatApp.Domain.Rooms;
using ChatApp.Domain.Users;
using ChatApp.Tests.Fixtures.Services;
using Moq;
using FluentAssertions;

namespace ChatApp.Tests.Application.Tests
{
    public class RoomServiceTests : IClassFixture<RoomServiceFixture>
    {
        private readonly RoomServiceFixture _fixture;
        public RoomServiceTests(RoomServiceFixture fixture)
        {
            _fixture = fixture;
        }

        [Theory]
        [InlineData(1, "Test Room 1")]
        public async Task GetRoom_ReturnsRoom(int roomId, string roomName)
        {
            // Arrange
            var room = new Room
            {
                Id = roomId,
                Name = roomName
            };

            _fixture.UnitOfWorkMock
                .Setup(u => u.GetRepository<Room, int>())
                .Returns(_fixture.RoomRepositoryMock.Object);

            _fixture.RoomRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(room);

            // Act
            var result = await _fixture.RoomService.GetRoom(roomId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(roomId, result.Id);
            Assert.Equal(roomName, result.Name);
        }

        [Theory]
        [InlineData("Test Room 1", 1, 1)]
        public async Task CreateRoom_ShouldReturnRoomId(string roomName, int creatorId, int expectedRoomId)
        {
            // Arrange
            _fixture.SetupCreateRoomTest(creatorId, expectedRoomId);

            // Act
            var result = await _fixture.RoomService.CreateRoom(roomName, creatorId.ToString());

            // Assert
            Assert.Equal(expectedRoomId, result);
            _fixture.RoomRepositoryMock.Verify(r => r.CreateAsync(It.IsAny<Room>()), Times.Once);
            _fixture.UnitOfWorkMock.Verify(u => u.SaveAsync(), Times.Once);
        }

        [Theory]
        [InlineData(1, "searchTerm", RoomColumnsSorting.Name, true, 1)]
        public async Task GetRoomsPageAsync_ReturnsExpectedResults(int userId, string searchTerm,
            RoomColumnsSorting column, bool asc, int pageNumber)
        {
            var data = new GridModelDto<RoomColumnsSorting>
            {
                Data = searchTerm,
                Column = column,
                Asc = asc,
                PageNumber = pageNumber,
                Sorting = true
            };
            var users = new List<User>
            {
                new() { Id = 1, UserName = "User1" },
                new() { Id = 2, UserName = "User2" },
                new() { Id = 3, UserName = "User3" },
            };
            var rooms = new List<Room>
            {
                new() { Id = 1, Name = "Room1", Users = new List<User> { users[0], users[1] } },
                new() { Id = 2, Name = "Room2", Users = new List<User> { users[1], users[2] }},
                new() { Id = 3, Name = "Room3", Users = new List<User> { users[0], users[2] }}
            }.AsQueryable();

            _fixture.SetupRoomsPageService(rooms);

            // Act
            var result = await _fixture.RoomService.GetRoomsPageAsync(userId, data);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<GridModelResponse<RoomDto>>(result);
            Assert.True(result.Items.Count() <= _fixture.PageSize);
            result.Items.Should().BeInAscendingOrder(p => p.Name);
        }
    }
}
