using ChatApp.Domain.DTOs.Http;
using ChatApp.Domain.DTOs.Http.Responses;
using ChatApp.Domain.DTOs.RoomDto;
using ChatApp.Domain.Enums;

namespace ChatApp.UI.Services.RoomApplicationService.Interfaces
{
    public interface IRoomApplicationService
    {
        Task<RoomDto> GetRoomAsync(string id);
        Task<AddRoomResponseDto> CreateRoomAsync(AddRoomDto dto);
        Task<GridModelResponse<RoomDto>> GetRoomsAsync(GridModelDto<RoomColumnsSorting> gridModelDto);
    }
}
