using ChatApp.Application.Services.MessageService.Interfaces;
using ChatApp.WebAPI.Controllers;
using Moq;

namespace ChatApp.Tests.Fixtures.Controllers
{
    public class MessageControllerFixture : IDisposable
    {
        public readonly Mock<IMessageService> MessageService;
        public readonly MessageController MessageController;
        public MessageControllerFixture()
        {
            MessageService = new Mock<IMessageService>();
            MessageController = new MessageController(MessageService.Object);
        }

        public void Dispose()
        {
            MessageService.Reset();
        }
    }
}
