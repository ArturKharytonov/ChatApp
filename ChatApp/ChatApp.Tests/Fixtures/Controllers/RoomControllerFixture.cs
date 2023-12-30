using ChatApp.Application.Services.FileService.Interface;
using ChatApp.Application.Services.RoomService.Interfaces;
using ChatApp.Application.Services.UserContext.Interfaces;
using ChatApp.UI.Services.OpenAiService;
using ChatApp.UI.Services.OpenAiService.Interfaces;
using ChatApp.WebAPI.Controllers;
using Moq;

namespace ChatApp.Tests.Fixtures.Controllers
{
    public class RoomControllerFixture : IDisposable
    {
        public readonly Mock<IUserContext> UserContext;
        public readonly Mock<IRoomService> RoomService;
        public readonly Mock<IOpenAiService> OpenAiService;
        public readonly Mock<IFileService> FileService;

        public readonly RoomController RoomController;

        public RoomControllerFixture()
        {
            UserContext = new Mock<IUserContext>();
            RoomService = new Mock<IRoomService>();
            OpenAiService = new Mock<IOpenAiService>();
            FileService = new Mock<IFileService>();
            RoomController = new RoomController(RoomService.Object, UserContext.Object, OpenAiService.Object, FileService.Object);
        }

        public void Dispose()
        {
            UserContext.Reset();
            RoomService.Reset();
            OpenAiService.Reset();
            FileService.Reset();
        }
    }
}
