using Blazored.LocalStorage;
using ChatApp.UI.Services.RtcService.Interfaces;
using ChatApp.UI.Services.SignalRService.Interfaces;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.IdentityModel.Tokens;

namespace ChatApp.UI.Shared
{
    public partial class MainLayout : LayoutComponentBase
    {
        [Inject]
        public AuthenticationStateProvider StateProvider { get; set; }

        [Inject]
        public ISignalRService SignalRService { get; set; }

        [Inject]
        public IWebRtcService RtcService { get; set; }

        protected override async Task OnInitializedAsync()
        {
            var authenticationResult = await StateProvider.GetAuthenticationStateAsync();
            if (authenticationResult.User.Identity!.IsAuthenticated)
            {
                await SignalRService.StartConnection();
                await RtcService.StartAsync();
            }
        }
    }
}
