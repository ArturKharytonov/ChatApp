using ChatApp.Domain.DTOs.Http.Requests.Common;
using ChatApp.Domain.DTOs.Http.Requests.Rooms;
using ChatApp.Domain.DTOs.Http.Responses.Common;
using ChatApp.Domain.DTOs.Http.Responses.Rooms;
using ChatApp.Domain.DTOs.RoomDto;
using ChatApp.Domain.Enums;

namespace ChatApp.UI.Services.RoomApplicationService.Interfaces
{
    public interface IRoomApplicationService
    {
        Task DeleteRoom(string roomId);
        Task<RoomDto> GetRoomByNameAsync(string name);
        Task<RoomDto> GetRoomAsync(string id);
        Task<AddRoomResponseDto> CreateRoomAsync(AddRoomDto dto);
        Task<GridModelResponse<RoomDto>> GetRoomsAsync(GridModelDto<RoomColumnsSorting> gridModelDto);
    }
}
