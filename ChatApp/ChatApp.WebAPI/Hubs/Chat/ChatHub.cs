using ChatApp.Domain.DTOs.MessageDto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace ChatApp.WebAPI.Hubs.Chat
{
    public class ChatHub : Hub
    {
        private const string _online = "Online";
        private const string _offline = "Offline";

        public async Task JoinRoom(string roomId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, roomId + _offline);
            await Groups.AddToGroupAsync(Context.ConnectionId, roomId);
        }
        public async Task LeaveRoom(string roomId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomId + _offline);
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomId + _online);
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomId);
        }
        public async Task AddToOnlineOfRoom(string roomId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomId + _offline);
            await Groups.AddToGroupAsync(Context.ConnectionId, roomId + _online);
        }
        public async Task AddToOfflineOfRoom(string roomId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomId + _online);
            await Groups.AddToGroupAsync(Context.ConnectionId, roomId + _offline);
        }
        public async Task SendMessage(string roomId, MessageDto message)
            => await Clients
                .Groups(roomId + _online)
                .SendAsync("ReceiveMessage", message);
        public async Task ChatNotificationAsync(string sender, string roomName, string roomId)
            => await Clients
                .Groups(roomId + _offline)
                .SendAsync("ReceiveNotification", sender, roomName, roomId);
    }
}
