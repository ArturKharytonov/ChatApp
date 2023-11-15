using ChatApp.Application.Services.QueryBuilder.Interfaces;
using ChatApp.Application.Services.UserService.Interfaces;
using ChatApp.Domain.DTOs.Http;
using ChatApp.Domain.DTOs.Http.Responses;
using ChatApp.Domain.DTOs.UserDto;
using ChatApp.Domain.Enums;
using ChatApp.Domain.Rooms;
using ChatApp.Domain.Users;
using ChatApp.Persistence.UnitOfWork.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace ChatApp.Application.Services.UserService
{
    public class UserService : IUserService
    {
        private readonly UserManager<User> _userManager;
        private readonly IQueryBuilder<User> _queryBuilder;
        private readonly IUnitOfWork _unitOfWork;
        private const int _pageSize = 5;

        public UserService(UserManager<User> userManager, IQueryBuilder<User> queryBuilder, IUnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _queryBuilder = queryBuilder;
            _unitOfWork = unitOfWork;
        }

        public async Task<User?> GetWithAll(string id)
        {
            return await _unitOfWork
                .GetRepository<User, int>()!
                .GetByIdAsync(int.Parse(id), u => u.Messages, u => u.Rooms);
        }

        public async Task<bool> AddUserToRoomAsync(AddUserToRoomDto userToRoom)
        {
            var user = await GetUserAsync(userToRoom.UserId);
            var room = await _unitOfWork
                .GetRepository<Room, int>()!
                .GetByIdAsync(int.Parse(userToRoom.RoomId!), r => r.Users);

            if (user is null ||
                room is null ||
                room.Users.Contains(user)) return false;

            room.Users.Add(user);
            await _unitOfWork.SaveAsync();
            return true;
        }

        public async Task<User?> GetUserAsync(string id)
            => await _userManager.FindByIdAsync(id);

        public async Task<bool> ChangePasswordAsync(string id, string newPassword, string currentPassword)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null || !await _userManager.CheckPasswordAsync(user, currentPassword))
                return false;

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, newPassword);
            return result.Succeeded;
        }

        public async Task<bool> UpdateUserAsync(UserDto user)
        {
            var existingUser = await _userManager.FindByIdAsync(user.Id.ToString());

            if(existingUser == null || (await DoesUsernameExistAsync(user.Username) &&
                                        !user.Username.Equals(existingUser.UserName))) 
                return false;

            existingUser.UserName = user.Username;
            existingUser.Email = user.Email;
            existingUser.PhoneNumber = user.PhoneNumber;

            var result = await _userManager.UpdateAsync(existingUser);

            return result.Succeeded;
        }

        public async Task<GridModelResponse<UserDto>> GetUsersPageAsync(GridModelDto<UserColumnsSorting> data)
        {
            var users = _userManager.Users.AsQueryable();

            if (!string.IsNullOrEmpty(data.Data))
                users = users.Where(_queryBuilder.SearchQuery(data.Data, Enum.GetNames(data.Column.GetType())));

            if (data.Sorting)
                users = _queryBuilder.OrderByQuery(users, data.Column.ToString(), data.Asc);

            var userInformation = users
                .Skip(data.PageNumber * _pageSize)
                .Take(_pageSize)
                .Select(user => new UserDto
                {
                    Id = user.Id,
                    Username = user.UserName,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                }).ToList();

            var totalCount = users.Count();

            return await Task.FromResult(new GridModelResponse<UserDto>
            {
                Items = userInformation,
                TotalCount = totalCount
            });
        }

        private async Task<bool> DoesUsernameExistAsync(string username)
        {
            var existingUser = await _userManager.FindByNameAsync(username);

            return existingUser != null;
        }
    }
}
