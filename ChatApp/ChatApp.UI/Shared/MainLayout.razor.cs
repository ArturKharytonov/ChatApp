using Blazored.LocalStorage;
using ChatApp.UI.Services.CustomAuthenticationState;
using ChatApp.UI.Services.RtcService.Interfaces;
using ChatApp.UI.Services.SignalRService.Interfaces;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.IdentityModel.Tokens;
using Radzen;

namespace ChatApp.UI.Shared
{
    public partial class MainLayout : LayoutComponentBase
    {
        [CascadingParameter]
        Task<AuthenticationState> AuthenticationState { get; set; }

        [Inject]
        public ISignalRService SignalRService { get; set; }

        [Inject]
        public IWebRtcService RtcService { get; set; }

        [Inject]
        public ILocalStorageService LocalStorageService { get; set; }
        protected override async Task OnInitializedAsync()
        {
            var auth = await AuthenticationState;
            var token = await LocalStorageService.GetItemAsync<string>("token");
            if (!token.IsNullOrEmpty())
            {
                await SignalRService.StartConnection();
                await RtcService.StartAsync();

                
            }
        }
    }
}
