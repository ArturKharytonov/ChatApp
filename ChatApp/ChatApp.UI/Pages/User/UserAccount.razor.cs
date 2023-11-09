using ChatApp.Application.UserApplicationService.Interfaces;
using ChatApp.Domain.DTOs.UserDto;
using Microsoft.AspNetCore.Components;


namespace ChatApp.UI.Pages.User
{
    public partial class UserAccount
    {
        public UserDto User = new();
        public string Message;
        [Inject] IUserApplicationService UserApplicationService { get; set; }
        [Inject] NavigationManager NavigationManager { get; set; }

        protected override async void OnInitialized()
        {
            User = await UserApplicationService.GetUserAsync();

            if (User == null)
                NavigationManager.NavigateTo("/logout");

            StateHasChanged();
        }
        private async Task OnSubmit()
        {
            var response = await UserApplicationService.UpdateUserAsync(User);
            Message = response.Message;
        }
    }
}
