using ChatApp.Domain.DTOs.Http.Responses;
using ChatApp.Domain.DTOs.Http;
using ChatApp.Domain.Enums;
using ChatApp.Domain.DTOs.RoomDto;

namespace ChatApp.Application.RoomApplicationService.Interfaces
{
    public interface IRoomApplicationService
    {
        Task<GridModelResponse<RoomDto>> GetRoomsAsync(GridModelDto<RoomColumnsSorting> gridModelDto);
    }
}
