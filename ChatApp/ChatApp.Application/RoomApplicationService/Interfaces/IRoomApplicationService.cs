using ChatApp.Domain.DTOs.Http;
using ChatApp.Domain.DTOs.Http.Responses;
using ChatApp.Domain.DTOs.RoomDto;
using ChatApp.Domain.Enums;

namespace ChatApp.Application.RoomApplicationService.Interfaces
{
    public interface IRoomApplicationService
    {
        Task<GridModelResponse<RoomDto>> GetRoomsAsync(GridModelDto<RoomColumnsSorting> gridModelDto);
    }
}
