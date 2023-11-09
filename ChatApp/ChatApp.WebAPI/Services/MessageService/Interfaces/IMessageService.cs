using ChatApp.Domain.DTOs.Http;
using ChatApp.Domain.DTOs.Http.Responses;
using ChatApp.Domain.DTOs.MessageDto;
using ChatApp.Domain.Enums;

namespace ChatApp.WebAPI.Services.MessageService.Interfaces
{
    public interface IMessageService
    {
        Task<GridModelResponse<MessageDto>> GetMessagePageAsync(GridModelDto<MessageColumnsSorting> data);
    }
}
