using ChatApp.Domain.DTOs.Http;
using ChatApp.Domain.DTOs.Http.Responses;


namespace ChatApp.Application.AuthenticationService.Interfaces
{
    public interface IAuthenticationService
    {
        Task<RegisterResponseDto> RegisterAsync(RegisterModelDto model);
        Task<LoginResponseDto> LoginAsync(LoginModelDto model);
        Task LogoutAsync();
    }
}
