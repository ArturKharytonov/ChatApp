using Microsoft.JSInterop;

namespace ChatApp.UI.Services.RtcService.Interfaces
{
    public interface IWebRtcService
    {
        event EventHandler<IJSObjectReference>? OnRemoteStreamAcquired;
        event Action OnCallAccepted;
        event Action OnHangUp;
        Task Join(string signalingChannel);
        Task<IJSObjectReference> StartLocalStream();
        Task Call();
        Task Hangup();
        Task SendOffer(string offer);
        Task StartAsync();
        Task AskForConfirmation(string channel, int senderId, int getterId);
        Task ConfirmationResponse(string channel, bool result);
    }
}
