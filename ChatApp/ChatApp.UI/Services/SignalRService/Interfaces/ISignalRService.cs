using ChatApp.Domain.DTOs.MessageDto;

namespace ChatApp.UI.Services.SignalRService.Interfaces
{
    public interface ISignalRService
    {
        public event Action<MessageDto> OnItemReceived;
        public event Action<MessageDto> OnItemDelete;
        public event Action<MessageDto> OnItemUpdate;

        Task DeleteMessageAsync(string id, MessageDto message);
        Task UpdateMessageAsync(string id, MessageDto messageToUpdate);
        Task AddToOnline(string id);
        Task AddToOffline(string id);
        Task StartConnection();
        Task StopConnection();
        Task SendMessage(string id, MessageDto message);
        Task AddToRoom(string id);
        Task DeleteFromRoom(string id);
    }
}
