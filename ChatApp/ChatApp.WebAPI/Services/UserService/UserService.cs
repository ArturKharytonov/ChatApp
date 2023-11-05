using System.Collections.Immutable;
using System.Linq;
using AutoMapper;
using ChatApp.Domain.DTOs.Http;
using ChatApp.Domain.DTOs.Http.Responses;
using ChatApp.Domain.DTOs.UserDto;
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
        private int _pageSize = 5;
        private readonly IMapper _mapper;
        public UserService(UserManager<User> userManager, IMapper mapper)
        {
            _userManager = userManager;
            _mapper = mapper;
        }

        public async Task<User?> GetUserAsync(string id)
            => await _userManager.FindByIdAsync(id);
        public async Task<bool> ChangePasswordAsync(string id, string newPassword, string currentPassword)
        {
            var user = await GetUserAsync(id);

            if (user == null || !await _userManager.CheckPasswordAsync(user, currentPassword)) 
                return false;

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result =  await _userManager.ResetPasswordAsync(user, token, newPassword);
            return result.Succeeded;
        }

        public async Task<bool> UpdateUserAsync(UserDto user)
        {
            var existingUser = await _userManager.FindByIdAsync(user.Id.ToString());
            if(existingUser == null || await DoesUsernameExistAsync(user.Username)) 
                return false;

            existingUser.UserName = user.Username;
            existingUser.Email = user.Email;
            existingUser.PhoneNumber = user.PhoneNumber;

            var result = await _userManager.UpdateAsync(existingUser);

            return result.Succeeded;
        }

        public int GetTotalCountOfUsers(string data)
        {
            var query = _userManager.Users
                .Where(user =>
                    user.UserName.Contains(data) ||
                    user.Email.Contains(data) ||
                    user.PhoneNumber.Contains(data));
            return query.Count();
        }

        public IEnumerable<UserDto> GetUsersByCredentials(GridModelDto data)
        {
            var user = _userManager.Users
                .Where(user =>
                    user.UserName.Contains(data.Data) ||
                    user.Email.Contains(data.Data) ||
                    user.PhoneNumber.Contains(data.Data))
                .Skip((data.PageNumber - 1) * _pageSize)
                .Take(_pageSize)
                .Select(user => new UserDto
                {
                    Id = user.Id,
                    Username = user.UserName,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                });

            return user.ToImmutableList();
        }

        private async Task<bool> DoesUsernameExistAsync(string username)
        {
            var existingUser = await _userManager.FindByNameAsync(username);

            return existingUser != null;
        }
    }
}
