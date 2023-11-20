using ChatApp.Domain.DTOs.MessageDto;

namespace ChatApp.WebAPI.Hubs.Interfaces
{
    public interface IChatHub
    {
        Task GetMessageDto(MessageDto message);
    }
}
