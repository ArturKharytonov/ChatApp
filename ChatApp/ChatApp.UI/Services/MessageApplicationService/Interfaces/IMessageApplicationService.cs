using ChatApp.Domain.DTOs.Http;
using ChatApp.Domain.DTOs.Http.Responses;
using ChatApp.Domain.DTOs.MessageDto;
using ChatApp.Domain.Enums;

namespace ChatApp.UI.Services.MessageApplicationService.Interfaces
{
    public interface IMessageApplicationService
    {
        Task DeleteMessageByIdAsync(int messageId);
        Task<UpdateMessageResponseDto> UpdateMessageAsync(MessageDto message);
        Task<List<MessageDto>> GetMessagesAsync(string roomId);
        Task<MessageDto> AddMessageAsync(AddMessageDto message);
        Task<GridModelResponse<MessageDto>> GetMessagesAsync(GridModelDto<MessageColumnsSorting> gridModelDto);
    }
}
