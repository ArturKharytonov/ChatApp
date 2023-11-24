using Blazored.LocalStorage;
using ChatApp.UI.Services.RtcService.Interfaces;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.JSInterop;
using System.IO;


namespace ChatApp.UI.Services.RtcService
{
    public class WebRtcService : IWebRtcService
    {
        private readonly NavigationManager _nav;
        private readonly IJSRuntime _js;
        private IJSObjectReference? _jsModule;
        private DotNetObjectReference<WebRtcService>? _jsThis;
        private readonly ILocalStorageService _localStorageService;
        private HubConnection? _hub;
        private string? _signalingChannel;
        public event EventHandler<IJSObjectReference>? OnRemoteStreamAcquired;


        public WebRtcService(IJSRuntime js, NavigationManager nav, ILocalStorageService localStorageService)
        {
            _js = js;
            _nav = nav;
            _localStorageService = localStorageService;
        }
        public async Task StartAsync()
        {
            _hub = new HubConnectionBuilder()
                .WithUrl(_nav.ToAbsoluteUri($"https://localhost:7223/callHub"), o => o.AccessTokenProvider =
                async () => await _localStorageService.GetItemAsync<string>("token"))
                .Build();
            _jsModule = await _js.InvokeAsync<IJSObjectReference>(
                "import", "/WebRtcService.cs.js");

            _jsThis = DotNetObjectReference.Create(this);
            await _jsModule.InvokeVoidAsync("initialize", _jsThis);


            _hub.On<string, string, string>("SignalWebRtc", async (signalingChannel, type, payload) =>
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

            _hub.On("ConfirmationResult", async () =>
            {
                await Call();
            });

            _hub.On("HangUp", async () =>
            {
                if (_jsModule == null) throw new InvalidOperationException();
                await _jsModule.InvokeVoidAsync("hangupAction");
            });
            await _hub.StartAsync();
        }
        public async Task Join(string signalingChannel)
        {
            _signalingChannel = signalingChannel;
            var hub = await GetHub();
            await hub.SendAsync("join", signalingChannel);
        }
        public async Task<IJSObjectReference> StartLocalStream()
        {
            if (_jsModule == null) throw new InvalidOperationException();
            var stream = await _jsModule.InvokeAsync<IJSObjectReference>("startLocalStream");
            return stream;
        }
        public async Task Call()
        {
            if (_jsModule == null) throw new InvalidOperationException();
            var offerDescription = await _jsModule.InvokeAsync<string>("callAction");
            await SendOffer(offerDescription);
        }
        public async Task Hangup()
        {
            if (_jsModule == null) throw new InvalidOperationException();
            await _jsModule.InvokeVoidAsync("hangupAction");

            var hub = await GetHub();
            await hub.SendAsync("HangUp", _signalingChannel);
        }
        private async Task<HubConnection> GetHub()
        {
            if (_hub != null)
                return _hub;

            await StartAsync();
            return _hub!;
        }
        public async Task ConfirmationResponse(string channel)
        {
            await _hub.SendAsync("ConfirmationResponse", channel);
        }

        [JSInvokable]
        public async Task SendOffer(string offer)
        {
            var hub = await GetHub();
            await hub.SendAsync("SignalWebRtc", _signalingChannel, "offer", offer);
        }

        [JSInvokable]
        public async Task SendAnswer(string answer)
        {
            var hub = await GetHub();
            await hub.SendAsync("SignalWebRtc", _signalingChannel, "answer", answer);
        }

        [JSInvokable]
        public async Task SendCandidate(string candidate)
        {
            var hub = await GetHub();
            await hub.SendAsync("SignalWebRtc", _signalingChannel, "candidate", candidate);
        }

        [JSInvokable]
        public async Task SetRemoteStream()
        {
            if (_jsModule == null) throw new InvalidOperationException();
            var stream = await _jsModule.InvokeAsync<IJSObjectReference>("getRemoteStream");
            OnRemoteStreamAcquired?.Invoke(this, stream);
        }

        //public async Task RegisterUserSignalGroupsAsync()
        //{
        //    var groupIds = await _accountService.GetUserSignalGroupsAsync();
        //    await _hub.SendAsync("RegisterMultipleGroupsAsync", groupIds.Result.SignalIdentifiers);
        //}
    }
}
