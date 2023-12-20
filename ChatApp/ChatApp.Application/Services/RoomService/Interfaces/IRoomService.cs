using ChatApp.Domain.DTOs.Http;
using ChatApp.Domain.DTOs.Http.Responses;
using ChatApp.Domain.DTOs.RoomDto;
using ChatApp.Domain.Enums;

namespace ChatApp.Application.Services.RoomService.Interfaces
{
    public interface IRoomService
    {
        Task<RoomDto> GetRoomByName(string name);
        Task<RoomDto> GetRoom(int id);
        Task<int?> CreateRoom(string name, string creatorId, string assistantId);
        Task<GridModelResponse<RoomDto>> GetRoomsPageAsync(int userId, GridModelDto<RoomColumnsSorting> data);
    }
}
