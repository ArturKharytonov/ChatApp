using ChatApp.Domain.DTOs.UserDto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace ChatApp.WebAPI.Hubs.Call
{
    [Authorize]
    public class CallHub : Hub
    {
        public async Task Join(string chanel)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, chanel);
        }
        public async Task HangUp(string chanel)
        {
            await Clients.OthersInGroup(chanel).SendAsync("HangUp");
        }

        public async Task SignalWebRtc(string channel, string type, string payload)
        {
            await Clients.OthersInGroup(channel).SendAsync("SignalWebRtc", channel, type, payload);
        }

        public async Task RegisterMultipleGroupsAsync(List<UserDto> users, int currentUserId)
        {
            var tasks = new List<Task>();
            foreach (var user in
                     users.Where(user => user.Id != currentUserId))
            {
                tasks.Add(Groups.AddToGroupAsync(Context.ConnectionId, $"video-{currentUserId}-{user.Id}"));
                tasks.Add(Groups.AddToGroupAsync(Context.ConnectionId, $"video-{user.Id}-{currentUserId}"));
            }
            await Task.WhenAll(tasks);
        }

        public async Task AskForConfirmation(string channel, int senderId, int getterId)
        {
            await Clients.OthersInGroup(channel).SendAsync("AskForConfirmation", channel, senderId, getterId);
        }

        public async Task ConfirmationResponse(string channel, bool isConfirmed)
        {
            await Clients.OthersInGroup(channel).SendAsync("ConfirmationResult", isConfirmed);
        }
    }
}
