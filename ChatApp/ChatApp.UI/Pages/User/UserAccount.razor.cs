using ChatApp.Domain.DTOs.UserDto;
using ChatApp.UI.Services.UserApplicationService.Interfaces;
using Microsoft.AspNetCore.Components;


namespace ChatApp.UI.Pages.User
{
    public partial class UserAccount
    {
        public UserDto User = new();
        public string Message;
        [Inject] IUserApplicationService UserApplicationService { get; set; }

        protected override async void OnInitialized()
        {
            User = await UserApplicationService.GetUserAsync();
            StateHasChanged();
        }
        private async Task OnSubmit()
        {
            var response = await UserApplicationService.UpdateUserAsync(User);
            Message = response.Message;
        }
    }
}
