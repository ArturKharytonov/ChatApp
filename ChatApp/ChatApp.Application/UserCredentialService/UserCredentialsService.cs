using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChatApp.Application.HttpClientPWAService;
using ChatApp.Application.HttpClientPWAService.Interfaces;
using ChatApp.Application.UserCredentialService.Interfaces;
using ChatApp.Domain.DTOs.Http;
using ChatApp.Domain.DTOs.Http.Responses;
using ChatApp.Domain.DTOs.UserDto;
using ChatApp.Domain.Users;

namespace ChatApp.Application.UserCredentialService
{
    public class UserCredentialsService : IUserCredentialsService
    {
        private readonly IHttpClientPwa _clientPwa;

        public UserCredentialsService(IHttpClientPwa clientPwa)
        {
            _clientPwa = clientPwa;
        }
        public async Task<ChangePasswordResponseDto> ChangePasswordAsync(ChangePasswordDto changePasswordDto)
        {
            var result = await _clientPwa.PostAsync<ChangePasswordDto, ChangePasswordResponseDto>(HttpClientPwa.ChangePasswordUrl, changePasswordDto);
            return result.Result;
        }
        public async Task<UserDto> GetUserAsync()
        {
            var result = await _clientPwa.GetAsync<UserDto>(HttpClientPwa.GetUser);
            return result.Result;
        }
        public async Task<UpdateUserCredentialResponse> UpdateUserAsync(UserDto user)
        {
            var result = await _clientPwa.PostAsync<UserDto, UpdateUserCredentialResponse>(HttpClientPwa.UpdateUser, user);
            return result.Result;
        }
        public async Task<GridModelResponse<UserDto>> GetUsersAsync(GridModelDto gridModelDto)
        {
            var result = await _clientPwa.GetAsync<GridModelResponse<UserDto>>(HttpClientPwa.GetUserByCredentials, gridModelDto);

            return result.Result;
        }
    }
}
