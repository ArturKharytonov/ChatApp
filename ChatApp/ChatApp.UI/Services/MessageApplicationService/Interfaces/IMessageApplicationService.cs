using ChatApp.Domain.DTOs.Http.Requests.Common;
using ChatApp.Domain.DTOs.Http.Requests.Messages;
using ChatApp.Domain.DTOs.Http.Responses.Common;
using ChatApp.Domain.DTOs.Http.Responses.Messages;
using ChatApp.Domain.DTOs.MessageDto;
using ChatApp.Domain.Enums;

namespace ChatApp.UI.Services.MessageApplicationService.Interfaces
{
    public interface IMessageApplicationService
    {
        Task DeleteAllMessagesFromRoomAsync(string roomId);
        Task DeleteMessageByIdAsync(int messageId);
        Task<UpdateMessageResponseDto> UpdateMessageAsync(MessageDto message);
        Task<List<MessageDto>> GetMessagesAsync(string roomId);
        Task<MessageDto> AddMessageAsync(AddMessageDto message);
        Task<GridModelResponse<MessageDto>> GetMessagesAsync(GridModelDto<MessageColumnsSorting> gridModelDto);
    }
}
