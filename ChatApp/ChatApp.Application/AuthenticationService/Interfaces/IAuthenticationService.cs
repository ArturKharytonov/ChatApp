using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChatApp.UI.ViewModels;
using ChatApp.UI.ViewModels.Responses;

namespace ChatApp.Application.AuthenticationService.Interfaces
{
    public interface IAuthenticationService
    {
        Task<RegisterResponse> RegisterAsync(RegisterModel model);
        Task<LoginResponse> LoginAsync(LoginModel model);
        Task LogoutAsync();
    }
}
