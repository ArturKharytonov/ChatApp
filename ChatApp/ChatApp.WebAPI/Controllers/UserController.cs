using System.Net;
using ChatApp.Domain.DTOs.Http;
using ChatApp.Domain.DTOs.Http.Responses;
using ChatApp.Domain.DTOs.UserDto;
using ChatApp.Domain.Enums;
using ChatApp.WebAPI.Services.UserService.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using IUserContext = ChatApp.WebAPI.Services.UserContext.Interfaces.IUserContext;

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

        [HttpGet]
        public async Task<IActionResult> GetUserAsync()
        {
            var userIdClaim = _userContext.GetUserId();
            if (string.IsNullOrEmpty(userIdClaim))
                return BadRequest(new ChangePasswordResponseDto { Success = false, Error = "Unable to retrieve user id from token." });
            return Ok(await _userService.GetUserAsync(userIdClaim));
        }

        [HttpPost("credentials")]
        public async Task<IActionResult> ChangeUserCredentials([FromBody] UserDto user)
        {
            if (await _userService.UpdateUserAsync(user))
                return Ok(new UpdateUserCredentialResponse { Message = "Credentials were updated" });
            return BadRequest(new UpdateUserCredentialResponse { Message = "UserName already exist" });
        }

        [HttpGet("page")]
        public IActionResult GetUsersPage([FromQuery] GridModelDto<UserColumnsSorting> userInput)
        {
            var users = _userService.GetUsersPage(userInput);

            return Ok(users);
        }
    }
}
