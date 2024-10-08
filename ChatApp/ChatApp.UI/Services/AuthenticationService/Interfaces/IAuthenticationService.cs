﻿using ChatApp.Domain.DTOs.Http.Requests.Users;
using ChatApp.Domain.DTOs.Http.Responses.Users;

namespace ChatApp.UI.Services.AuthenticationService.Interfaces
{
    public interface IAuthenticationService
    {
        Task<ChangePasswordResponseDto> ChangePasswordAsync(ChangePasswordDto changePasswordDto);
        Task<RegisterResponseDto> RegisterAsync(RegisterModelDto model);
        Task<LoginResponseDto> LoginAsync(LoginModelDto model);
        Task LogoutAsync();
    }
}
