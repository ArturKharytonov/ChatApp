using System;
using System.Collections.Generic;
using ChatApp.Application.Services.QueryBuilder.Interfaces;
using ChatApp.Application.Services.RoomService;
using ChatApp.Domain.DTOs.Http;
using ChatApp.Domain.DTOs.RoomDto;
using ChatApp.Domain.Enums;
using ChatApp.Domain.Rooms;
using ChatApp.Domain.Users;
using ChatApp.Persistence.Common.Interfaces;
using ChatApp.Persistence.UnitOfWork.Interfaces;
using Microsoft.AspNetCore.Identity;
using Moq;

namespace ChatApp.Tests.Application.Tests
{
    public class RoomServiceTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IQueryBuilder<RoomDto>> _queryBuilderMock;
        private readonly Mock<UserManager<User>> _userManagerMock;
        private readonly Mock<IRepository<Room, int>> _roomRepositoryMock;
        private readonly RoomService _roomService;
        private const int _pageSize = 5;
        public RoomServiceTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _queryBuilderMock = new Mock<IQueryBuilder<RoomDto>>();
            _userManagerMock = new Mock<UserManager<User>>(
                Mock.Of<IUserStore<User>>(),
                null, null, null, null, null, null, null, null);
            _roomRepositoryMock = new Mock<IRepository<Room, int>>();
            _roomService = new RoomService(_queryBuilderMock.Object, _unitOfWorkMock.Object, _userManagerMock.Object);
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

            _unitOfWorkMock
                .Setup(u => u.GetRepository<Room, int>())
                .Returns(_roomRepositoryMock.Object);

            _roomRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(room);

            // Act
            var result = await _roomService.GetRoom(roomId);

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
            var user = new User { Id = creatorId, UserName = "TestUser" };

            _unitOfWorkMock.Setup(u => u.GetRepository<Room, int>())
                .Returns(_roomRepositoryMock.Object);

            _roomRepositoryMock.Setup(r => r.GetAllAsQueryableAsync())
                .ReturnsAsync(new List<Room>().AsQueryable());

            _userManagerMock.Setup(u => u.FindByIdAsync(creatorId.ToString()))
                .ReturnsAsync(user);

            _roomRepositoryMock.Setup(r => r.CreateAsync(It.IsAny<Room>()))
                .Callback<Room>(r => r.Id = expectedRoomId)
                .Returns(Task.CompletedTask);

            // Act
            var result = await _roomService.CreateRoom(roomName, creatorId.ToString());

            // Assert
            Assert.Equal(expectedRoomId, result);
            _roomRepositoryMock.Verify(r => r.CreateAsync(It.IsAny<Room>()), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveAsync(), Times.Once);
        }
    }
}
