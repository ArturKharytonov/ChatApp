using ChatApp.Domain.DTOs.Http;
using Microsoft.AspNetCore.Components;
using IAuthenticationService = ChatApp.UI.Services.AuthenticationService.Interfaces.IAuthenticationService;

namespace ChatApp.UI.Pages.Authentication
{
    public partial class SignUp
    {
        [Inject]
        protected IAuthenticationService _authenticationService { get; set; }
        [Inject]
        private NavigationManager _navigationManager { get; set; }

        private readonly RegisterModelDto _registerModel = new();

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
