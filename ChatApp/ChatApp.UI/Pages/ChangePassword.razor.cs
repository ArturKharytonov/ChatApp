using ChatApp.Application.AuthenticationService.Interfaces;
using ChatApp.Application.UserCredentialService.Interfaces;
using ChatApp.Domain.DTOs.Http;
using Microsoft.AspNetCore.Components;

namespace ChatApp.UI.Pages
{
    public partial class ChangePassword
    {
        private readonly ChangePasswordDto _changePasswordDto = new();
        [Inject] private NavigationManager _navigationManager { get; set; }
        private string Error;
        private bool _showErrors;
        private bool _showMessage;
        [Inject] private IAuthenticationService AuthenticationService { get; set; }

        public async Task ChangePasswordAsync()
        {
            _showErrors = false;
            _showMessage = false;

            var result = await AuthenticationService.ChangePasswordAsync(_changePasswordDto);
            if (result.Success)
                _showMessage = true;
            
            else
            {
                _showErrors = true;
                Error = result.Error;
            }
        }
    }
}
