using ChatApp.Domain.DTOs.Http;
using ChatApp.UI.Services.SignalRService.Interfaces;
using ChatApp.UI.Services.UserApplicationService.Interfaces;
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
        [Inject]
        private IUserApplicationService _userApplicationService { get; set; }

        [CascadingParameter]
        protected ISignalRService SignalRService { get; set; }

        private readonly LoginModelDto _loginModel = new();

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
                await SignalRService.StartConnection();
                var groups = await _userApplicationService.GetAllUserGroups();

                if (groups is not null &&
                    groups.GroupsId.Count > 0)
                {
                    foreach (var groupId in groups.GroupsId)
                        await SignalRService.AddToOffline(groupId);
                }

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
