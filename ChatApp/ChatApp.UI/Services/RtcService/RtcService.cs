using ChatApp.UI.Services.RtcService.Interfaces;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.JSInterop;

namespace ChatApp.UI.Services.RtcService
{
    public class RtcService : IRtcService
    {
        private readonly NavigationManager _nav;
        private readonly IJSRuntime _js;

        private IJSObjectReference? _jsModule;
        private DotNetObjectReference<RtcService>? _jsThis;
        private HubConnection? _hub;
        private string? _signalingChannel;
        public event EventHandler<IJSObjectReference>? OnRemoteStreamAcquired;

        public RtcService(IJSRuntime js, NavigationManager nav)
        {
            _js = js;
            _nav = nav;
        }

        public async Task Join(string signalingChannel)
        {
            if (_signalingChannel != null) throw new InvalidOperationException();
            _signalingChannel = signalingChannel;
            var hub = await GetHub();
            await hub.SendAsync("join", signalingChannel);
            _jsModule = await _js.InvokeAsync<IJSObjectReference>(
                "import", "/js/WebRtcService.cs.js");
            _jsThis = DotNetObjectReference.Create(this);
            await _jsModule.InvokeVoidAsync("initialize", _jsThis);
        }

        private async Task<HubConnection> GetHub()
        {

            if (_hub != null) return _hub;

            var hub = new HubConnectionBuilder()
                .WithUrl(_nav.ToAbsoluteUri("/messagehub"))
                .Build();

            hub.On<string, string, string>("SignalWebRtc", async (signalingChannel, type, payload) =>
            {
                if (_jsModule == null) throw new InvalidOperationException();

                if (_signalingChannel != signalingChannel) return;
                switch (type)
                {
                    case "offer":
                        await _jsModule.InvokeVoidAsync("processOffer", payload);
                        break;
                    case "answer":
                        await _jsModule.InvokeVoidAsync("processAnswer", payload);
                        break;
                    case "candidate":
                        await _jsModule.InvokeVoidAsync("processCandidate", payload);
                        break;
                }
            });
        }
}
