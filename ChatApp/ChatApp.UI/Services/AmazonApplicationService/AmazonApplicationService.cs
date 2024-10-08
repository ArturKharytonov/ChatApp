using ChatApp.Domain.DTOs.Http.Responses.Amazon;
using ChatApp.UI.Services.AmazonApplicationService.Interfaces;
using ChatApp.UI.Services.HttpClientPWAService;
using ChatApp.UI.Services.HttpClientPWAService.Interfaces;

namespace ChatApp.UI.Services.AmazonApplicationService
{
    public class AmazonApplicationService : IAmazonApplicationService
    {
        private readonly IHttpClientPwa _clientPwa;
        
        public AmazonApplicationService(IHttpClientPwa clientPwa)
        {
            _clientPwa = clientPwa;
        }

        public async Task<AmazonResponseDto> GetPageAsync(string good)
        {
            var res = await _clientPwa.GetAsync<AmazonResponseDto>(HttpClientPwa.GetAmazonPage + "name=" + good);
            return res.Result;
        }
    }
}
