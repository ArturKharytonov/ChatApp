using ChatApp.Application.Services.AmazonService.Interfaces;
using ChatApp.Domain.DTOs.Http.Responses;
using ChatApp.Domain.DTOs.Http.Responses.Amazon;
using ChatApp.UI.Services.OpenAiService.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace ChatApp.WebAPI.Controllers;

[Route("api/amazon")]
[ApiController]
[Authorize]
public class AmazonController : ControllerBase
{
    private readonly IAmazonService AmazonService;
    private readonly IOpenAiService OpenAiService;
    public AmazonController(IAmazonService amazonService, IOpenAiService openAiService)
    {
        AmazonService = amazonService;
        OpenAiService = openAiService;
    }

    [HttpGet]
    public async Task<IActionResult> GetPage([FromQuery] string name)
    {
        //BUG: HERE BUG, appeared later, maybe html was changed
        var html = await AmazonService.GetPageAsync(name);
        var products = new List<AmazonProductDto>();

        if (html is null) 
            return BadRequest(new AmazonResponseDto { List = products });

        var count = 0;
        foreach (var node in html.Elements())
        {
            if(count >= 3)
                break;

            var res = await OpenAiService.ChatCompletionAsync(node.InnerHtml);

            var product = JsonConvert.DeserializeObject<AmazonProductDto>(res);

            if (product != null)
                products.Add(product);
            count++;
        }

        //var tasks = html.Elements().Select(async node =>
        //{
        //    var res = await OpenAiService.ChatCompletionAsync(node.OuterHtml);

        //    var product = JsonConvert.DeserializeObject<AmazonProductDto>(res);

        //    if (product != null)
        //        products.Add(product);
        //});

        //await Task.WhenAll(tasks);

        return Ok(new AmazonResponseDto{List = products});
    }
}