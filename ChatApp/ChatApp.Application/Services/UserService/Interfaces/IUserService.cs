using ChatApp.Domain.DTOs.Http.Requests.Common;
using ChatApp.Domain.DTOs.Http.Requests.Users;
using ChatApp.Domain.DTOs.Http.Responses.Common;
using ChatApp.Domain.DTOs.UserDto;
using ChatApp.Domain.Enums;
using ChatApp.Domain.Users;

namespace ChatApp.Application.Services.UserService.Interfaces
{
    public interface IUserService
    {
        Task DeleteUsersFromRoom(int roomId);
        Task<User?> GetWithAll(string id);
        Task<bool> AddUserToRoomAsync(AddUserToRoomDto userToRoom);
        Task<bool> ChangePasswordAsync(string id, string newPassword, string currentPassword);
        Task<bool> UpdateUserAsync(UserDto user);
        Task<User?> GetUserAsync(string id);
        Task<GridModelResponse<UserDto>> GetUsersPageAsync(GridModelDto<UserColumnsSorting> data);
        Task<List<UserDto>> GetAllUsers();
    }
}
