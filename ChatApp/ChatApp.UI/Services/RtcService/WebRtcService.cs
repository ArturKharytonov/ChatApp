using Blazored.LocalStorage;
using ChatApp.UI.Services.RtcService.Interfaces;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.JSInterop;
using Radzen;


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
        public event Action OnCallAccepted;
        public event Action OnHangUp;

        private readonly DialogService _dialogService;

        public WebRtcService(IJSRuntime js,
            NavigationManager nav,
            ILocalStorageService localStorageService,
            DialogService dialogService)
        {
            _js = js;
            _nav = nav;
            _localStorageService = localStorageService;
            _dialogService = dialogService;
        }

        public async Task StartAsync()
        {
            var apiUrl = Environment.GetEnvironmentVariable("API_URL");

            await Console.Out.WriteLineAsync($"{apiUrl}/callHub");

            _hub = new HubConnectionBuilder()
                .WithUrl($"{apiUrl}/callHub", o => o.AccessTokenProvider =
                async () => await _localStorageService.GetItemAsync<string>("token"))
                .Build();

            _jsModule = await _js.InvokeAsync<IJSObjectReference>(
                "import", "./WebRtcService.cs.js");

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

            _hub.On<string, int, int>("AskForConfirmation", async (channel, senderId, getterId) =>
            {
                var result = await _dialogService.Confirm("Incoming call!", $"Call in {channel}", new()
                {
                    OkButtonText = "Answer",
                    CancelButtonText = "Decline",
                });
                if(result!.Value)
                    _nav.NavigateTo($"/roomcall?senderId={senderId}&getterId={getterId}&requestCall=true");
                else
                    await ConfirmationResponse(channel, result.Value);
            });

            _hub.On<bool>("ConfirmationResult", async (isConfirmed) =>
            {
                if (isConfirmed)
                {
                    OnCallAccepted.Invoke();
                    await Call();
                    return;
                }
                await _dialogService.Alert("It seems that user is busy", "Call declined!");
            });

            _hub.On("HangUp", async () =>
            {
                if (_jsModule == null) throw new InvalidOperationException();
                await _jsModule.InvokeVoidAsync("hangupAction");
                OnHangUp.Invoke();
            });

            await _hub.StartAsync();
            await RegisterUserSignalGroupsAsync();
        }
        public async Task Join(string signalingChannel)
        {
            _signalingChannel = signalingChannel;
            var hub = await GetHub();
            await hub.SendAsync("Join", signalingChannel);
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
        public async Task ConfirmationResponse(string channel, bool result)
        {
            await _hub.SendAsync("ConfirmationResponse", channel, result);
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

        public async Task RegisterUserSignalGroupsAsync()
        {
            //var groupIds = new List<int>(); // get all groups ids where user exist
            await _hub!.SendAsync("RegisterMultipleGroupsAsync");
        }

        public async Task AskForConfirmation(string channel, int senderId, int getterId)
        {
            var hub = await GetHub();
            await hub.SendAsync("AskForConfirmation", channel, senderId, getterId);
        }
    }
}
