using ChatApp.Domain.DTOs.Http;
using ChatApp.Domain.DTOs.Http.Responses;
using ChatApp.Domain.DTOs.UserDto;
using ChatApp.Domain.Users;

namespace ChatApp.WebAPI.Services.UserService.Interfaces
{
    public interface IUserService
    {
        Task<bool> UpdateUserAsync(UserDto user);
        Task<bool> ChangePasswordAsync(string id, string newPassword, string currentPassword);
        Task<User?> GetUserAsync(string id);
        IEnumerable<UserDto> GetUsersByCredentials(GridModelDto data);
        int GetTotalCountOfUsers(string data);
    }
}
