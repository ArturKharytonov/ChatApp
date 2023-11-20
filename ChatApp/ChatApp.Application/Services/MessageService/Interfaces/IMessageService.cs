using ChatApp.Domain.DTOs.Http;
using ChatApp.Domain.DTOs.Http.Responses;
using ChatApp.Domain.DTOs.MessageDto;
using ChatApp.Domain.Enums;

namespace ChatApp.Application.Services.MessageService.Interfaces
{
    public interface IMessageService
    {
        Task DeleteMessageAsync(int messageId);
        Task<bool> UpdateMessageAsync(MessageDto message);
        Task<IEnumerable<MessageDto>> GetMessagesFromChat(string roomId);
        Task<MessageDto> AddMessageAsync(AddMessageDto addMessageDto);
        Task<GridModelResponse<MessageDto>> GetMessagePageAsync(GridModelDto<MessageColumnsSorting> data);
    }
}
