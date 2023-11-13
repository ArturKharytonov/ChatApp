using ChatApp.Domain.DTOs.Http;
using Microsoft.AspNetCore.Components;
using Radzen;
using IAuthenticationService = ChatApp.UI.Services.AuthenticationService.Interfaces.IAuthenticationService;

namespace ChatApp.UI.Pages.Authentication
{
    public partial class Login
    {
        [Inject]
        protected IAuthenticationService _authenticationService { get; set; }
        [Inject]
        private NavigationManager _navigationManager { get; set; }

        private LoginModelDto _loginModel = new();

        private bool _showErrors;
        private string Error = "";
        private async Task SubmitForm(LoginArgs args)
        {
            _loginModel.UserName = args.Username;
            _loginModel.Password = args.Password;


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

        private void OnRegister()
            => _navigationManager.NavigateTo("/signup");
    }
}
