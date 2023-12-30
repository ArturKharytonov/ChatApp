using HtmlAgilityPack;

namespace ChatApp.Application.Services.AmazonService.Interfaces;

public interface IAmazonService
{
    Task<HtmlNodeCollection?> GetPageAsync(string good);
}