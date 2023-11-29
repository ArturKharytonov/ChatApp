using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace ChatApp.WebAPI.Hubs.Call
{
    [Authorize]
    public class CallHub : Hub
    {
        private readonly IEnumerable<string> calls = new List<string>();

        public async Task Join(string channel)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, channel);
            await Clients.OthersInGroup(channel).SendAsync("Join", Context.ConnectionId);
        }
        public async Task HangUp(string channel)
        {
            await Clients.OthersInGroup(channel).SendAsync("HangUp");
        }

        public async Task SignalWebRtc(string channel, string type, string payload)
        {
            await Clients.OthersInGroup(channel).SendAsync("SignalWebRtc", channel, type, payload);
        }

        public async Task RegisterMultipleGroupsAsync() //IEnumerable<(int, int)> groupsId
        {
            var groupsId = new List<(int, int)> { (1, 3), (3, 1) };
            var tasks = groupsId
                .Select(id => $"video-{id.Item1}-{id.Item2}")
                .Select(groupName => Groups.AddToGroupAsync(Context.ConnectionId, groupName))
                .ToList();
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
