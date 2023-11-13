using System.Web;
using ChatApp.Domain.DTOs.Http;
using ChatApp.Domain.DTOs.Http.Responses;
using ChatApp.Domain.DTOs.MessageDto;
using ChatApp.Domain.Enums;
using ChatApp.UI.Services.HttpClientPWAService;
using ChatApp.UI.Services.HttpClientPWAService.Interfaces;
using ChatApp.UI.Services.MessageApplicationService.Interfaces;

namespace ChatApp.UI.Services.MessageApplicationService
{
    public class MessageApplicationService : IMessageApplicationService
    {
        private readonly IHttpClientPwa _clientPwa;

        public MessageApplicationService(IHttpClientPwa httpClientPwa)
        {
            _clientPwa = httpClientPwa;
        }

        public async Task<List<MessageDto>> GetMessagesAsync(string roomId)
        {
            var result = await _clientPwa.GetAsync<List<MessageDto>>(HttpClientPwa.GetAllMessagesFromChat + roomId);
            return result.Result;
        }

        public async Task<MessageDto> AddMessageAsync(AddMessageDto message)
        {
            var result = await _clientPwa.PostAsync<AddMessageDto, MessageDto>(HttpClientPwa.AddMessage, message);
            return result.Result;
        }

        public async Task<GridModelResponse<MessageDto>> GetMessagesAsync(GridModelDto<MessageColumnsSorting> gridModelDto)
        {
            var queryString = GenerateQueryString(gridModelDto);

            var result = await _clientPwa.GetAsync<GridModelResponse<MessageDto>>(HttpClientPwa.GetMessagesBySearch + queryString);

            return result.Result;
        }

        private static string GenerateQueryString(GridModelDto<MessageColumnsSorting> gridModelDto)
        {
            var queryParameters = new System.Collections.Specialized.NameValueCollection
            {
                ["data"] = gridModelDto.Data,
                ["pageNumber"] = gridModelDto.PageNumber.ToString(),
                ["column"] = gridModelDto.Column.ToString(),
                ["asc"] = gridModelDto.Asc.ToString(),
                ["sorting"] = gridModelDto.Sorting.ToString()
            };

            var queryString = string.Join("&",
                queryParameters.AllKeys.Select(key =>
                    $"{HttpUtility.UrlEncode(key)}={HttpUtility.UrlEncode(queryParameters[key])}"
                )
            );

            return queryString;
        }
    }
}
