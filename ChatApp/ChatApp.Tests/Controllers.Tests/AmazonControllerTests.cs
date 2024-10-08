using ChatApp.Domain.DTOs.Http.Responses.Amazon;
using ChatApp.Tests.Fixtures.Controllers;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace ChatApp.Tests.Controllers.Tests;

public class AmazonControllerTests : IClassFixture<AmazonControllerFixture>
{
    private readonly AmazonControllerFixture _fixture;

    public AmazonControllerTests(AmazonControllerFixture fixture)
    {
        _fixture = fixture;
    }

    [Theory]
    [InlineData("<html><div>Product 1</div></html>", 1)]
    public async Task GetPage_ReturnsExpectedResult(string amazonServiceResult, int expectedProductCount)
    {
        _fixture.AmazonService.Setup(x => x.GetPageAsync(It.IsAny<string>())).ReturnsAsync(() =>
        {
            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(amazonServiceResult);
            return htmlDocument.DocumentNode.SelectNodes(
                "//div[text()='Product 1']");
        });

        _fixture.OpenAiService.Setup(x => x.ChatCompletionAsync(It.IsAny<string>()))
            .ReturnsAsync("{\"image\": \"image_url_here\", \"name\": \"Product 1\"}");

        // Act
        var result = await _fixture.AmazonController.GetPage("test");

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var amazonResponseDto = Assert.IsType<AmazonResponseDto>(okResult.Value);
        Assert.Equal(expectedProductCount, amazonResponseDto.List.Count);
    }
}