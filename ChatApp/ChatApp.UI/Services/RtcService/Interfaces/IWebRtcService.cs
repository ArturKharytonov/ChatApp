using Microsoft.JSInterop;

namespace ChatApp.UI.Services.RtcService.Interfaces
{
    public interface IWebRtcService
    {
        public event EventHandler<IJSObjectReference>? OnRemoteStreamAcquired;
        Task StartAsync();
        Task Join(string signalingChannel);
        Task<IJSObjectReference> StartLocalStream();
        Task Call();
        Task Hangup();
        Task ConfirmationResponse(string channel);
    }
}
