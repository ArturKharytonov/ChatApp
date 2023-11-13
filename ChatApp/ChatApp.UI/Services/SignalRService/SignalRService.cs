using ChatApp.Domain.DTOs.MessageDto;
using ChatApp.UI.Services.SignalRService.Interfaces;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using Radzen;

namespace ChatApp.UI.Services.SignalRService
{
    //[Authorize]
    // : IAsyncDisposable
    public class SignalRService : ISignalRService
    {
        private HubConnection? _hubConnection;

        public event Action<MessageDto> OnItemReceived;

        private readonly NotificationService _notificationService;
        private readonly NavigationManager _navigationManager;

        public SignalRService(NotificationService notificationService, NavigationManager navigationManager)
        {
            _notificationService = notificationService;
            _navigationManager = navigationManager;
        }

        public async Task StartConnection()
        {
            if (_hubConnection is not null)
                return;

            _hubConnection = new HubConnectionBuilder()
                .WithUrl("https://localhost:7223/chatHub")
                .Build();

            _hubConnection.On<MessageDto>("ReceiveMessage", item => OnItemReceived?.Invoke(item));
            _hubConnection.On<string, string, string>("ReceiveNotification", (username, roomName, roomId) =>
            {
                _notificationService.Notify(new NotificationMessage
                {
                    Summary = $"Chat: {roomName}. Message from: {username}",
                    Severity = NotificationSeverity.Info,
                    Duration = 4000,
                    Click = (message) => _navigationManager.NavigateTo($"/chat/{roomId}")
                });
            });
            await _hubConnection.StartAsync();
        }

        public async Task SendMessage(string id, MessageDto message)
        {
            if (_hubConnection is null)
                return;

            await _hubConnection.SendAsync("ChatNotificationAsync", message.SenderUsername, message.RoomName, id);
            await _hubConnection.SendAsync("SendMessage", id, message);
        }

        public async Task AddToOnline(string id)
        {
            await _hubConnection.SendAsync("AddToOnlineOfRoom", id);
        }
        public async Task AddToOffline(string id)
        {
            await _hubConnection.SendAsync("AddToOfflineOfRoom", id);
        }
    }
}
