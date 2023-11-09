using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using ChatApp.Application.HttpClientPWAService;
using ChatApp.Application.HttpClientPWAService.Interfaces;
using ChatApp.Application.MessageApplicationService.Interfaces;
using ChatApp.Domain.DTOs.Http;
using ChatApp.Domain.DTOs.Http.Responses;
using ChatApp.Domain.DTOs.MessageDto;
using ChatApp.Domain.DTOs.RoomDto;
using ChatApp.Domain.Enums;

namespace ChatApp.Application.MessageApplicationService
{
    public class MessageApplicationService : IMessageApplicationService
    {
        private readonly IHttpClientPwa _clientPwa;

        public MessageApplicationService(IHttpClientPwa httpClientPwa)
        {
            _clientPwa = httpClientPwa;
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
