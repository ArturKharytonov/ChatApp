using ChatApp.Application.AuthenticationService.Interfaces;
using ChatApp.Domain.DTOs.Http;
using Microsoft.AspNetCore.Components;

namespace ChatApp.UI.Pages
{
    public partial class Login
    {
        [Inject]
        protected IAuthenticationService _authenticationService { get; set; }
        [Inject]
        private NavigationManager _navigationManager { get; set; }

        private readonly LoginModelDto _loginModel = new();

        private bool _showErrors;
        private string Error = "";
        private async Task SubmitForm()
        {
            _showErrors = false;
            var result = await _authenticationService.LoginAsync(_loginModel);

            if (result.Success)
            {
                _navigationManager.NavigateTo("/");
            }
            else
            {
                Error = result.Error;
                _showErrors = true;
            }
        }
    }
}
