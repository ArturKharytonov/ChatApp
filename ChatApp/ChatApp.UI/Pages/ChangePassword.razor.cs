using ChatApp.Application.UserCredentialService.Interfaces;
using ChatApp.Domain.DTOs.Http;
using Microsoft.AspNetCore.Components;

namespace ChatApp.UI.Pages
{
    public partial class ChangePassword
    {
        private readonly ChangePasswordDto _changePasswordDto = new();
        private string Error;
        private bool _showErrors;
        private bool _showMessage;
        [Inject] private IUserCredentialsService CredentialsService { get; set; }

        public async Task ChangePasswordAsync()
        {
            _showErrors = false;
            _showMessage = false;

            var result = await CredentialsService.ChangePasswordAsync(_changePasswordDto);
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
