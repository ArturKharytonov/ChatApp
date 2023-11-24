using Microsoft.AspNetCore.SignalR;

namespace ChatApp.WebAPI.Hubs.Call
{
    public class CallHub : Hub
    {
        public async Task Join(string channel)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, channel);
            await Clients.OthersInGroup(channel).SendAsync("Join", Context.ConnectionId);
        }

        public async Task HangUp(string channel)
        {
            await Clients.OthersInGroup(channel).SendAsync("HangUp", Context.ConnectionId);
        }

        public async Task SignalWebRtc(string channel, string type, string payload)
        {
            await Clients.OthersInGroup(channel).SendAsync("SignalWebRtc", channel, type, payload);
        }

        public async Task RegisterMultipleGroupsAsync(IEnumerable<int> groupsId)
        {
            var tasks = new List<Task>();
            string groupName;
            foreach (int id in groupsId)
            {
                groupName = $"video-{id}";
                tasks.Add(Groups.AddToGroupAsync(Context.ConnectionId, groupName));
            }
            await Task.WhenAll(tasks);
        }
        public async Task ConfirmationResponse(string channel)
        {
            await Clients.OthersInGroup(channel).SendAsync("ConfirmationResult");
        }
    }
}
