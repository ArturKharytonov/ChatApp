using ChatApp.Domain.DTOs.Http.Requests.Common;
using ChatApp.Domain.DTOs.Http.Responses.Common;
using ChatApp.Domain.DTOs.RoomDto;
using ChatApp.Domain.Enums;

namespace ChatApp.Application.Services.RoomService.Interfaces
{
    public interface IRoomService
    {
        Task<bool> DoesRoomExist(string roomName);
        Task DeleteRoom(int roomId);
        Task<RoomDto> GetRoomByName(string name);
        Task<RoomDto> GetRoom(int id);
        Task<int?> CreateRoom(string name, string creatorId, string assistantId);
        Task<GridModelResponse<RoomDto>> GetRoomsPageAsync(int userId, GridModelDto<RoomColumnsSorting> data);
    }
}
