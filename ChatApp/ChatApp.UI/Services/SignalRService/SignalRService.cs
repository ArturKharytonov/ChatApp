﻿using Blazored.LocalStorage;
using ChatApp.Domain.DTOs.MessageDto;
using ChatApp.Domain.DTOs.UserDto;
using ChatApp.UI.Services.SignalRService.Interfaces;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using Radzen;

namespace ChatApp.UI.Services.SignalRService
{
    public class SignalRService : ISignalRService
    {
        private HubConnection? _hubConnection;

        public event Action<MessageDto> OnItemReceived;
        public event Action<MessageDto> OnItemDelete;
        public event Action<MessageDto> OnItemUpdate;
        public event Action<List<UserDto>> OnParticipantsUpdate;

        private readonly NotificationService _notificationService;
        private readonly NavigationManager _navigationManager;
        private readonly ILocalStorageService _localStorageService;

        public SignalRService(NotificationService notificationService, NavigationManager navigationManager, ILocalStorageService localStorageService)
        {
            _notificationService = notificationService;
            _navigationManager = navigationManager;
            _localStorageService = localStorageService;
        }

        public async Task StartConnection()
        {
            var apiUrl = Environment.GetEnvironmentVariable("API_URL");
            _hubConnection = new HubConnectionBuilder()
                .WithUrl($"{apiUrl}/chatHub", o =>
                    o.AccessTokenProvider = async () => await _localStorageService.GetItemAsync<string>("token"))
                .Build();

            _hubConnection.On<MessageDto>("ReceiveMessage", item => OnItemReceived?.Invoke(item));
            _hubConnection.On<MessageDto>("OnMessageDelete", item => OnItemDelete?.Invoke(item));
            _hubConnection.On<MessageDto>("OnMessageUpdate", item => OnItemUpdate?.Invoke(item));
            
            _hubConnection.On<List<UserDto>>("OnUpdateParticipants", list => OnParticipantsUpdate?.Invoke(list));

            _hubConnection.On<string, string>("CallStarted", (chatName,userName) =>
            {
                _notificationService.Notify(new NotificationMessage
                {
                    Summary = $"{userName} entered to {chatName} meeting",
                    Severity = NotificationSeverity.Info,
                    Duration = 4000
                });
            });

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
        public async Task StopConnection()
        {
            if (_hubConnection is not null)
                await _hubConnection.StopAsync();
        }
        public async Task SendMessage(string id, MessageDto message)
        {
            if (_hubConnection is null)
                return;

            await _hubConnection.SendAsync("ChatNotificationAsync", message.SenderUsername, message.RoomName, id);
            await _hubConnection.SendAsync("SendMessage", id, message);
        }
        public async Task DeleteMessageAsync(string id, MessageDto message)
        {
            if (_hubConnection is null)
                return;
            await _hubConnection.SendAsync("DeleteMessageAsync", id, message);
        }
        public async Task UpdateMessageAsync(string id, MessageDto messageToUpdate)
        {
            if (_hubConnection is null)
                return;
            await _hubConnection.SendAsync("UpdateMessageAsync", id, messageToUpdate);
        }


        //call
        public async Task NotifyAboutCall(string roomId, string roomName, string userName)
        {
            if (_hubConnection is null)
                return;
            await _hubConnection.SendAsync("NotifyAboutCall", roomId, roomName, userName);
        }

        public async Task AddToCall(string roomName, UserDto user)
        {
            if (_hubConnection is null)
                return;
            await _hubConnection.SendAsync("JoinCall", roomName, user);
        }

        public async Task LeaveFromCall(string roomName, UserDto user)
        {
            if (_hubConnection is null)
                return;
            await _hubConnection.SendAsync("LeaveCall", roomName, user);
        }
        //call

        // on one group layer
        public async Task AddToOnline(string id)
        {
            if (_hubConnection is null)
                return;
            await _hubConnection.SendAsync("AddToOnlineOfRoom", id);
        }
        public async Task AddToOffline(string id)
        {
            if (_hubConnection is null)
                return;
            await _hubConnection.SendAsync("AddToOfflineOfRoom", id);
        }
        // on one group layer

        //maybe useless methods
        public async Task AddToRoom(string id)
        {
            if (_hubConnection is null)
                return;
            await _hubConnection.SendAsync("JoinRoom", id);
        }
        public async Task DeleteFromRoom(string id)
        {
            if (_hubConnection is null)
                return;
            await _hubConnection.SendAsync("LeaveRoom", id);
        }
    }
}
