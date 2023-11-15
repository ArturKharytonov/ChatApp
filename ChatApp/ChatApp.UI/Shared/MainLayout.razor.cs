using Blazored.LocalStorage;
using ChatApp.UI.Services.SignalRService.Interfaces;
using Microsoft.AspNetCore.Components;
using Microsoft.IdentityModel.Tokens;

namespace ChatApp.UI.Shared
{
    public partial class MainLayout : LayoutComponentBase
    {
        [Inject]
        public ISignalRService SignalRService { get; set; }
        [Inject] public ILocalStorageService LocalStorageService { get; set; }

        protected override async Task OnInitializedAsync()
        {
            var token = await LocalStorageService.GetItemAsync<string>("token");
            if (!token.IsNullOrEmpty())
                await SignalRService.StartConnection();
        }
    }
}
