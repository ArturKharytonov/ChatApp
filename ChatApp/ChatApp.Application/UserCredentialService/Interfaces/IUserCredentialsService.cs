﻿using ChatApp.Domain.DTOs.Http;
using ChatApp.Domain.DTOs.Http.Responses;
using ChatApp.Domain.DTOs.UserDto;
using ChatApp.Domain.Enums;

namespace ChatApp.Application.UserCredentialService.Interfaces
{
    public interface IUserCredentialsService
    {
        Task<UserDto> GetUserAsync();
        Task<GridModelResponse<UserDto>> GetUsersAsync(GridModelDto<UserColumnsSorting> gridModelDto);
        Task<UpdateUserCredentialResponse> UpdateUserAsync(UserDto user);
    }
}
