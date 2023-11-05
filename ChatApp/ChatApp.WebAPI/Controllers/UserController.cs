using ChatApp.Domain.DTOs.Http;
using ChatApp.Domain.DTOs.Http.Responses;
using ChatApp.Domain.DTOs.UserDto;
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

        [HttpPost("change_password")] // change on put
        public async Task<IActionResult> ChangePasswordAsync([FromBody] ChangePasswordDto changePassword)
        {
            var userIdClaim = _userContext.GetUserId();
            if (string.IsNullOrEmpty(userIdClaim))
                return BadRequest(new ChangePasswordResponseDto{ Success = false, Error = "Unable to retrieve user id from token." });
            
            if (await _userService.ChangePasswordAsync(userIdClaim, changePassword.NewPassword, changePassword.CurrentPassword))
                return Ok(new ChangePasswordResponseDto{Success = true});

            return BadRequest(new ChangePasswordResponseDto{Success = false, Error = "Wrong current password"});
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
            return BadRequest(new UpdateUserCredentialResponse { Message = "Username already exist" });
        }

        [HttpGet("by_credentials")]
        public IActionResult GetUsersByCredentials([FromQuery] GridModelDto userInput)
        {
            var users = _userService.GetUsersByCredentials(userInput);

            var totalCount = _userService.GetTotalCountOfUsers(userInput.Data);
            return Ok(new GridModelResponse
            {
                Users = users,
                TotalCount = totalCount
            });
        }
    }
}
