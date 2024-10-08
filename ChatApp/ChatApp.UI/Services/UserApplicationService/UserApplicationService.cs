﻿using System.Web;
using ChatApp.Domain.DTOs.Http.Requests.Common;
using ChatApp.Domain.DTOs.Http.Requests.Users;
using ChatApp.Domain.DTOs.Http.Responses.Common;
using ChatApp.Domain.DTOs.Http.Responses.Users;
using ChatApp.Domain.DTOs.UserDto;
using ChatApp.Domain.Enums;
using ChatApp.UI.Services.HttpClientPWAService;
using ChatApp.UI.Services.HttpClientPWAService.Interfaces;
using ChatApp.UI.Services.UserApplicationService.Interfaces;

namespace ChatApp.UI.Services.UserApplicationService
{
    public class UserApplicationService : IUserApplicationService
    {
        private readonly IHttpClientPwa _clientPwa;

        public UserApplicationService(IHttpClientPwa clientPwa)
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
            var queryString = GenerateQueryString(gridModelDto);

            var result = await _clientPwa.GetAsync<GridModelResponse<UserDto>>(HttpClientPwa.GetUsersBySearch + queryString);

            return result.Result;
        }
        public async Task<AddUserToChatResponseDto> AddUserToGroup(AddUserToRoomDto user)
        {
            var result = await _clientPwa.PostAsync<AddUserToRoomDto, AddUserToChatResponseDto>(HttpClientPwa.AddUserToRoom, user);

            return result.Result;
        }
        public async Task<UserGroupsResponseDto?> GetAllUserGroups()
        {
            var result = await _clientPwa.GetAsync<UserGroupsResponseDto>(HttpClientPwa.GetUserRooms);

            return result.Result;
        }

        public async Task<AllUsersResponseDto> GetAllUsers()
        {
            var result = await _clientPwa.GetAsync<AllUsersResponseDto>(HttpClientPwa.GetAllUsers);

            return result.Result;
        }

        private static string GenerateQueryString(GridModelDto<UserColumnsSorting> gridModelDto)
        {
            var queryParameters = new System.Collections.Specialized.NameValueCollection
            {
                ["data"] = gridModelDto.Data,
                ["pageNumber"] = gridModelDto.PageNumber.ToString(),
                ["column"] = gridModelDto.Column.ToString(),
                ["asc"] = gridModelDto.Asc.ToString(),
                ["sorting"] = gridModelDto.Sorting.ToString()
            };

            var queryString = string.Join("&",
                queryParameters.AllKeys.Select(key =>
                    $"{HttpUtility.UrlEncode(key)}={HttpUtility.UrlEncode(queryParameters[key])}"
                )
            );

            return queryString;
        }
    }
}
