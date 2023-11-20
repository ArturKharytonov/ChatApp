using System.Web;
using ChatApp.Domain.DTOs.Http;
using ChatApp.Domain.DTOs.Http.Responses;
using ChatApp.Domain.DTOs.RoomDto;
using ChatApp.Domain.Enums;
using ChatApp.UI.Services.HttpClientPWAService;
using ChatApp.UI.Services.HttpClientPWAService.Interfaces;
using ChatApp.UI.Services.RoomApplicationService.Interfaces;

namespace ChatApp.UI.Services.RoomApplicationService
{
    public class RoomApplicationService : IRoomApplicationService
    {
        private readonly IHttpClientPwa _clientPwa;

        public RoomApplicationService(IHttpClientPwa clientPwa)
        {
            _clientPwa = clientPwa;
        }
        public async Task<RoomDto> GetRoomAsync(string id)
        {
            var result = await _clientPwa.GetAsync<RoomDto>(HttpClientPwa.GetRoom + "id=" + id);
            return result.Result;
        }
        public async Task<AddRoomResponseDto> CreateRoomAsync(AddRoomDto dto)
        {
            var result = await _clientPwa.GetAsync<AddRoomResponseDto>(HttpClientPwa.CreateRoom + "roomName=" + dto.Name);
            return result.Result;
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
