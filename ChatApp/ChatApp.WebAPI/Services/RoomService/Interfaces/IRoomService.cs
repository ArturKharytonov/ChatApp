using ChatApp.Domain.DTOs.Http;
using ChatApp.Domain.DTOs.Http.Responses;
using ChatApp.Domain.DTOs.RoomDto;
using ChatApp.Domain.Enums;

namespace ChatApp.WebAPI.Services.RoomService.Interfaces
{
    public interface IRoomService
    {
        Task<GridModelResponse<RoomDto>> GetRoomsPageAsync(GridModelDto<RoomColumnsSorting> data);
    }
}
