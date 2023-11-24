using ChatApp.Domain.Rooms;
using ChatApp.Tests.Fixtures.Services;
using Moq;

namespace ChatApp.Tests.Application.Tests
{
    public class RoomServiceTests : IClassFixture<RoomServiceFixture>
    {
        private readonly RoomServiceFixture _fixture;
        public RoomServiceTests()
        {
            _fixture = new RoomServiceFixture();
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



    }
}
