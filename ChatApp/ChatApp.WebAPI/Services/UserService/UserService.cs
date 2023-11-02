using ChatApp.Domain.Users;
using ChatApp.Persistence.UnitOfWork.Interfaces;
using ChatApp.UI.Pages;
using ChatApp.WebAPI.Services.UserService.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace ChatApp.WebAPI.Services.UserService
{
    public class UserService : IUserService
    {
        private readonly UserManager<User> _userManager;

        public UserService(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public async Task<bool> ChangePasswordAsync(string id, string newPassword, string currentPassword)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null || !await _userManager.CheckPasswordAsync(user, currentPassword)) 
                return false;

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result =  await _userManager.ResetPasswordAsync(user, token, newPassword);
            return result.Succeeded;
        }
    }
}
