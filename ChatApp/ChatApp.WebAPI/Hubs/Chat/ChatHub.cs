using ChatApp.Domain.DTOs.MessageDto;
using ChatApp.Domain.DTOs.UserDto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace ChatApp.WebAPI.Hubs.Chat
{
    [Authorize]
    public class ChatHub : Hub
    {
        private const string _online = "Online";
        private const string _offline = "Offline";
        private static Dictionary<string, List<UserDto>> callParticipants = new Dictionary<string, List<UserDto>>();

        //call
        public async Task NotifyAboutCall(string roomId, string roomName, string userName)
        {
            await Clients
            .Groups(roomId + _online)
                .SendAsync("CallStarted", roomName, userName);
            await Clients
                .Groups(roomId + _offline)
                .SendAsync("CallStarted", roomName, userName);
        }
        public async Task JoinCall(string roomName, UserDto user)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, roomName);

            if (!callParticipants.ContainsKey(roomName))
                callParticipants[roomName] = new List<UserDto>();

            if (callParticipants[roomName].Any(u =>
                    u.Id == user.Id) == false)
            {
                callParticipants[roomName].Add(user);

                await Clients
                    .Groups(roomName)
                    .SendAsync("OnUpdateParticipants", callParticipants[roomName]);
            }
        }
        public async Task LeaveCall(string roomName, UserDto user)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomName);

            if (callParticipants.ContainsKey(roomName))
            {
                callParticipants[roomName] = callParticipants[roomName].Where(u => u.Id != user.Id).ToList();


                await Clients
                    .Groups(roomName)
                    .SendAsync("OnUpdateParticipants", callParticipants[roomName]);
            }
        }
        //call

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

        public async Task DeleteMessageAsync(string roomId, MessageDto message)
            => await Clients
                .Groups(roomId + _online)
                .SendAsync("OnMessageDelete", message);
        
        public async Task UpdateMessageAsync(string roomId, MessageDto message)
            => await Clients
                .Groups(roomId + _online)
                .SendAsync("OnMessageUpdate", message);
        
        public async Task ChatNotificationAsync(string sender, string roomName, string roomId)
            => await Clients
                .Groups(roomId + _offline)
                .SendAsync("ReceiveNotification", sender, roomName, roomId);
    }
}
