using ChatApp.Domain.DTOs.MessageDto;

namespace ChatApp.UI.Services.SignalRService.Interfaces
{
    public interface ISignalRService
    {
        public event Action<MessageDto> OnItemReceived;
        Task AddToOnline(string id);
        Task AddToOffline(string id);
        Task StartConnection();
        Task StopConnection();
        Task SendMessage(string id, MessageDto message);
        Task AddToRoom(string id);
        Task DeleteFromRoom(string id);
    }
}
