using ChatApp.Application.Services.AmazonService.Interfaces;
using ChatApp.UI.Services.OpenAiService.Interfaces;
using ChatApp.WebAPI.Controllers;
using Moq;

namespace ChatApp.Tests.Fixtures.Controllers;

public class AmazonControllerFixture : IDisposable
{
    public readonly Mock<IAmazonService> AmazonService;
    public readonly Mock<IOpenAiService> OpenAiService;
    public readonly AmazonController AmazonController;
    public AmazonControllerFixture()
    {
        AmazonService = new Mock<IAmazonService>();
        OpenAiService = new Mock<IOpenAiService>();
        AmazonController = new AmazonController(AmazonService.Object, OpenAiService.Object);
    }
    public void Dispose() { }
}