using ChatApp.Domain.DTOs.Http;
using ChatApp.Domain.DTOs.Http.Responses;

namespace ChatApp.Application.UserCredentialService.Interfaces
{
    public interface IUserCredentialsService
    {
        Task<ChangePasswordResponseDto> ChangePasswordAsync(ChangePasswordDto changePasswordDto);
    }
}
