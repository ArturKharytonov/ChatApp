using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using ChatApp.Application.HttpClientPWAService;
using ChatApp.Application.HttpClientPWAService.Interfaces;
using ChatApp.Application.RoomApplicationService.Interfaces;
using ChatApp.Domain.DTOs.Http;
using ChatApp.Domain.DTOs.Http.Responses;
using ChatApp.Domain.DTOs.RoomDto;
using ChatApp.Domain.DTOs.UserDto;
using ChatApp.Domain.Enums;

namespace ChatApp.Application.RoomApplicationService
{
    public class RoomApplicationService : IRoomApplicationService
    {
        private readonly IHttpClientPwa _clientPwa;

        public RoomApplicationService(IHttpClientPwa clientPwa)
        {
            _clientPwa = clientPwa;
        }

        public async Task<GridModelResponse<RoomDto>> GetRoomsAsync(GridModelDto<RoomColumnsSorting> gridModelDto)
        {
            var queryString = GenerateQueryString(gridModelDto);

            var result = await _clientPwa.GetAsync<GridModelResponse<RoomDto>>(HttpClientPwa.GetRoomsBySearch + queryString);

            return result.Result;
        }
        private static string GenerateQueryString(GridModelDto<RoomColumnsSorting> gridModelDto)
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
