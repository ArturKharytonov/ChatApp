using ChatApp.Application.HttpClientPWAService;
using ChatApp.Application.HttpClientPWAService.Interfaces;
using ChatApp.Application.UserCredentialService.Interfaces;
using ChatApp.Domain.DTOs.Http;
using ChatApp.Domain.DTOs.Http.Responses;
using ChatApp.Domain.DTOs.UserDto;
using ChatApp.Domain.Enums;

namespace ChatApp.Application.UserCredentialService
{
    public class UserCredentialsService : IUserCredentialsService
    {
        private readonly IHttpClientPwa _clientPwa;

        public UserCredentialsService(IHttpClientPwa clientPwa)
        {
            _clientPwa = clientPwa;
        }

        public async Task<UserDto> GetUserAsync()
        {
            var result = await _clientPwa.GetAsync<UserDto>(HttpClientPwa.GetUser);
            return result.Result;
        }
        public async Task<UpdateUserCredentialResponse> UpdateUserAsync(UserDto user)
        {
            var result = await _clientPwa.PostAsync<UserDto, UpdateUserCredentialResponse>(HttpClientPwa.UpdateUserCredentials, user);
            return result.Result;
        }

        public async Task<GridModelResponse<UserDto>> GetUsersAsync(GridModelDto<UserColumnsSorting> gridModelDto)
        {
            var link = $"{HttpClientPwa.GetUsersBySearch}?" +
                       $"data={gridModelDto.Data}&" +
                       $"pageNumber={gridModelDto.PageNumber}&" +
                       $"column={gridModelDto.Column}&" +
                       $"asc={gridModelDto.Asc}&" +
                       $"sorting={gridModelDto.Sorting}";
            var result = await _clientPwa.GetAsync<GridModelResponse<UserDto>>(link);

            return result.Result;
        } // add maybe to paging service
    }
}
