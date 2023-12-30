using ChatApp.Application.Services.AmazonService.Interfaces;
using HtmlAgilityPack;
using System.Net;

namespace ChatApp.Application.Services.AmazonService;

public class AmazonService : IAmazonService
{
    private const string url = "https://www.amazon.in/s?k=";

    public async Task<HtmlNodeCollection?> GetPageAsync(string good)
    {
        var request = (HttpWebRequest)WebRequest.Create(url + good);
        request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

        using var response = (HttpWebResponse)request.GetResponse();

        if (response.StatusCode == HttpStatusCode.OK)
        {
            await using var stream = response.GetResponseStream();
            using var reader = new StreamReader(stream);
            var content = await reader.ReadToEndAsync();
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(content);

            return htmlDoc.DocumentNode.SelectNodes("//*[@id=\"search\"]/div[1]/div[1]/div/span[1]/div[1]//*[contains(@class, 'sg-col-inner')]");
        }

        return null;
    }
}