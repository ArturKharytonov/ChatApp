﻿using ChatApp.Application.UserCredentialService.Interfaces;
using ChatApp.Domain.DTOs.UserDto;
using Microsoft.AspNetCore.Components;


namespace ChatApp.UI.Pages
{
    public partial class UserAccount
    {
        public UserDto User = new ();
        public string Message;
        [Inject] IUserCredentialsService UserCredentialsService { get; set; }
        [Inject] NavigationManager NavigationManager { get; set; }

        protected override async void OnInitialized()
        {
            User = await UserCredentialsService.GetUserAsync();

            if (User == null)
                NavigationManager.NavigateTo("/logout");

            StateHasChanged();
        }
        private async Task OnSubmit()
        {
            var response = await UserCredentialsService.UpdateUserAsync(User);
            Message = response.Message;
        }
    }
}
