using ChatApp.Application.Services.QueryBuilder.Interfaces;
using ChatApp.Application.Services.UserService.Interfaces;
using ChatApp.Domain.DTOs.Http.Requests.Common;
using ChatApp.Domain.DTOs.Http.Requests.Users;
using ChatApp.Domain.DTOs.Http.Responses.Common;
using ChatApp.Domain.DTOs.UserDto;
using ChatApp.Domain.Enums;
using ChatApp.Domain.Mappers.Users;
using ChatApp.Domain.Messages;
using ChatApp.Domain.Rooms;
using ChatApp.Domain.Users;
using ChatApp.Persistence.UnitOfWork.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

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

        public async Task DeleteUsersFromRoom(int roomId)
        {
            var rooms = await _unitOfWork.GetRepository<Room, int>()!.GetAllAsQueryableAsync();
            var room = await rooms
                .Include(x => x.Users)
                .FirstOrDefaultAsync(r => r.Id == roomId);

            if (room == null || !room.Users.Any()) return;

            foreach (var user in room.Users)
                await DeleteUserAsync(user.Id);
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

            if (existingUser == null || (await DoesUsernameExistAsync(user.Username) &&
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
                .Select(user => user.ToUserDto()).ToList();

            var totalCount = users.Count();

            return await Task.FromResult(new GridModelResponse<UserDto>
            {
                Items = userInformation,
                TotalCount = totalCount
            });
        }

        public async Task<List<UserDto>> GetAllUsers()
        {
            var users = await _userManager.Users.ToListAsync();
            return users.Select(user => user.ToUserDto()).ToList();
        }

        private async Task<bool> DoesUsernameExistAsync(string username)
        {
            var existingUser = await _userManager.FindByNameAsync(username);

            return existingUser != null;
        }

        private async Task DeleteUserAsync(int userId)
        {
            var repo = _unitOfWork.GetRepository<User, int>()!;
            await repo.DeleteAsync(userId);
            await _unitOfWork.SaveAsync();
        }

    }
}
