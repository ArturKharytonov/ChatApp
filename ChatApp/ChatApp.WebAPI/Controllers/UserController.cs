﻿using ChatApp.Application.Services.UserContext.Interfaces;
using ChatApp.Application.Services.UserService.Interfaces;
using ChatApp.Domain.DTOs.Http.Requests.Common;
using ChatApp.Domain.DTOs.Http.Requests.Users;
using ChatApp.Domain.DTOs.Http.Responses;
using ChatApp.Domain.DTOs.Http.Responses.Rooms;
using ChatApp.Domain.DTOs.Http.Responses.Users;
using ChatApp.Domain.DTOs.UserDto;
using ChatApp.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChatApp.WebAPI.Controllers
{
    [Route("api/user")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IUserContext _userContext;
        public UserController(IUserService userService, IUserContext userContext)
        {
            _userService = userService;
            _userContext = userContext;
        }

        [HttpPost]
        public async Task<IActionResult> AddUserToGroup([FromBody] AddUserToRoomDto userToRoomDto)
        {
            if (await _userService.AddUserToRoomAsync(userToRoomDto))
                return Ok(new AddRoomResponseDto { WasAdded = true });

            return Ok(new AddRoomResponseDto{WasAdded = false});
        }

        [HttpGet("rooms")]
        public async Task<IActionResult> GetUserGroupsAsync()
        {
            var userIdClaim = _userContext.GetUserId();
            var user = await _userService.GetWithAll(userIdClaim!);

            if(user is null)
                return NotFound();
            
            var userGroupsResponseDto = new UserGroupsResponseDto();

            if (user.Rooms.Count <= 0) 
                return Ok(userGroupsResponseDto);


            foreach (var room in user.Rooms)
                userGroupsResponseDto.GroupsId.Add(room.Id.ToString());
            
            return Ok(userGroupsResponseDto);
        }


        [HttpGet]
        public async Task<IActionResult> GetUserAsync()
        {
            var userIdClaim = _userContext.GetUserId();
            if (string.IsNullOrEmpty(userIdClaim))
                return BadRequest(new ChangePasswordResponseDto { Success = false, Error = "Unable to retrieve user id from token." });
            return Ok(await _userService.GetUserAsync(userIdClaim));
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllUsers()
        {
            return Ok(new AllUsersResponseDto{Users = await _userService.GetAllUsers() });
        }

        [HttpPost("credentials")]
        public async Task<IActionResult> ChangeUserCredentials([FromBody] UserDto user)
        {
            if (await _userService.UpdateUserAsync(user))
                return Ok(new UpdateUserCredentialResponse { Message = "Credentials were updated" });
            return BadRequest(new UpdateUserCredentialResponse { Message = "UserName already exist" });
        }

        [HttpGet("page")]
        public async Task<IActionResult> GetUsersPage([FromQuery] GridModelDto<UserColumnsSorting> userInput)
        {
            var users = await _userService.GetUsersPageAsync(userInput);

            return Ok(users);
        }
    }
}
