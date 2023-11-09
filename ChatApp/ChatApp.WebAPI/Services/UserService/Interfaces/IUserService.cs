using ChatApp.Domain.DTOs.Http;
using ChatApp.Domain.DTOs.Http.Responses;
using ChatApp.Domain.DTOs.UserDto;
using ChatApp.Domain.Enums;
using ChatApp.Domain.Users;

namespace ChatApp.WebAPI.Services.UserService.Interfaces
{
    public interface IUserService
    {
        Task<bool> ChangePasswordAsync(string id, string newPassword, string currentPassword);
        Task<bool> UpdateUserAsync(UserDto user);
        Task<User?> GetUserAsync(string id);
        Task<GridModelResponse<UserDto>> GetUsersPageAsync(GridModelDto<UserColumnsSorting> data);
    }
}
