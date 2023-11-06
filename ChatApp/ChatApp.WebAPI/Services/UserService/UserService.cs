using System.Collections.Immutable;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using AutoMapper;
using ChatApp.Domain.DTOs.Http;
using ChatApp.Domain.DTOs.Http.Responses;
using ChatApp.Domain.DTOs.UserDto;
using ChatApp.Domain.Users;
using ChatApp.Persistence.UnitOfWork.Interfaces;
using ChatApp.UI.Pages;
using ChatApp.WebAPI.Services.UserService.Interfaces;
using LinqKit;
using Microsoft.AspNetCore.Identity;

namespace ChatApp.WebAPI.Services.UserService
{
    public class UserService : IUserService
    {
        private readonly UserManager<User> _userManager;
        private const int _pageSize = 5;
        public UserService(UserManager<User> userManager)
        {
            _userManager = userManager;
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
            var searchPredicate = CreateSearchPredicate(data.Data);

            var users = _userManager.Users
                .Where(searchPredicate)
                .Skip((data.PageNumber - 1) * _pageSize)
                .Take(_pageSize)
                .Select(user => new UserDto
                {
                    Id = user.Id,
                    Username = user.UserName,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                });

            return users.ToImmutableList();
        }
        private static Expression<Func<User, bool>> CreateSearchPredicate(string input)
        {
            var predicate = PredicateBuilder.New<User>(false);

            predicate = predicate.Or(user =>
                user.UserName.Contains(input) ||
                user.Email.Contains(input) ||
                user.PhoneNumber.Contains(input));

            return predicate;
        }
        private async Task<bool> DoesUsernameExistAsync(string username)
        {
            var existingUser = await _userManager.FindByNameAsync(username);

            return existingUser != null;
        }
    }
}
