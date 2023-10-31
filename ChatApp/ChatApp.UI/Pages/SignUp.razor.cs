using ChatApp.Application.AuthenticationService.Interfaces;
using ChatApp.UI.ViewModels;
using Microsoft.AspNetCore.Components;
namespace ChatApp.UI.Pages
{
    public partial class SignUp
    {
        [Inject]
        protected IAuthenticationService _authenticationService { get; set; }
        [Inject]
        private NavigationManager _navigationManager { get; set; }

        private readonly RegisterModel _registerModel = new();

        private bool _showErrors;

        private IEnumerable<string>? _errors;

        private async Task SubmitForm()
        {
            _showErrors = false;

            var result = await _authenticationService.RegisterAsync(_registerModel);
            if (result.Successful)
            {
                _navigationManager.NavigateTo("/login");
            }
            else
            {
                _errors = result.Errors;
                _showErrors = true;
            }
        }
    }
}
