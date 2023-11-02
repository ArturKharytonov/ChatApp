using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChatApp.Application.HttpClientPWAService;
using ChatApp.Application.HttpClientPWAService.Interfaces;
using ChatApp.Application.UserCredentialService.Interfaces;
using ChatApp.Domain.DTOs.Http;
using ChatApp.Domain.DTOs.Http.Responses;

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
            var result = await _clientPwa.PostAsync<ChangePasswordDto, ChangePasswordResponseDto>(HttpClientPwa.ChangePasswordUrl, changePasswordDto, true);
            return result.Result;
        }
    }
}
