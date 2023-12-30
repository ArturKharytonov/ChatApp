using ChatApp.Domain.DTOs.Http.Responses.Amazon;
using HtmlAgilityPack;

namespace ChatApp.UI.Services.AmazonApplicationService.Interfaces
{
    public interface IAmazonApplicationService
    {
        Task<AmazonResponseDto> GetPageAsync(string good);
    }
}
