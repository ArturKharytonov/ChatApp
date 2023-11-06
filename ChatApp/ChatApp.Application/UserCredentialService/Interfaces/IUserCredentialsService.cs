using ChatApp.Domain.DTOs.Http;
using ChatApp.Domain.DTOs.Http.Responses;
using ChatApp.Domain.DTOs.UserDto;

namespace ChatApp.Application.UserCredentialService.Interfaces
{
    public interface IUserCredentialsService
    {
        Task<UserDto> GetUserAsync();
        Task<GridModelResponse<UserDto>> GetUsersAsync(GridModelDto gridModelDto);
        Task<UpdateUserCredentialResponse> UpdateUserAsync(UserDto user);
        Task<ChangePasswordResponseDto> ChangePasswordAsync(ChangePasswordDto changePasswordDto);
    }
}
