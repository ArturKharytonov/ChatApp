using ChatApp.UI.Services.SignalRService;
using ChatApp.UI.Services.SignalRService.Interfaces;
using Microsoft.AspNetCore.Components;
using Radzen;

namespace ChatApp.UI.Shared
{
    public partial class MainLayout : LayoutComponentBase
    {
        [Inject]
        protected NotificationService NotificationService { get; set; }

        [Inject]
        public ISignalRService SignalRService { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await SignalRService.StartConnection();
        }
    }
}
