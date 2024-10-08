using ChatApp.Domain.DTOs.Http.Requests.Common;
using ChatApp.Domain.DTOs.Http.Requests.Users;
using ChatApp.Domain.DTOs.Http.Responses.Common;
using ChatApp.Domain.DTOs.Http.Responses.Users;
using ChatApp.Domain.DTOs.UserDto;
using ChatApp.Domain.Enums;

namespace ChatApp.UI.Services.UserApplicationService.Interfaces
{
    public interface IUserApplicationService
    {
        Task<AddUserToChatResponseDto> AddUserToGroup(AddUserToRoomDto user);
        Task<UserGroupsResponseDto?> GetAllUserGroups();
        Task<UserDto> GetUserAsync();
        Task<GridModelResponse<UserDto>> GetUsersAsync(GridModelDto<UserColumnsSorting> gridModelDto);
        Task<UpdateUserCredentialResponse> UpdateUserAsync(UserDto user);
        Task<AllUsersResponseDto> GetAllUsers();
    }
}
