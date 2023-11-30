using ChatApp.Application.Services.RoomService.Interfaces;
using ChatApp.Application.Services.UserContext.Interfaces;
using ChatApp.WebAPI.Controllers;
using Moq;

namespace ChatApp.Tests.Fixtures.Controllers
{
    public class RoomControllerFixture : IDisposable
    {
        public readonly Mock<IUserContext> UserContext;
        public readonly Mock<IRoomService> RoomService;
        public readonly RoomController RoomController;

        public RoomControllerFixture()
        {
            UserContext = new Mock<IUserContext>();
            RoomService = new Mock<IRoomService>();
            RoomController = new RoomController(RoomService.Object, UserContext.Object);
        }

        public void Dispose()
        {
            UserContext.Reset();
            RoomService.Reset();
        }
    }
}
