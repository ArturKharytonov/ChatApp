using ChatApp.Domain.DTOs.Http.Requests.Users;
using Microsoft.AspNetCore.Components;
using IAuthenticationService = ChatApp.UI.Services.AuthenticationService.Interfaces.IAuthenticationService;

namespace ChatApp.UI.Pages.User
{
    public partial class ChangePassword
    {
        private readonly ChangePasswordDto _changePasswordDto = new();
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
