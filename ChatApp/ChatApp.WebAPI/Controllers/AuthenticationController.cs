using ChatApp.Application.Services.JwtHandler.Interfaces;
using ChatApp.Application.Services.UserContext.Interfaces;
using ChatApp.Application.Services.UserService.Interfaces;
using ChatApp.Domain.DTOs.Http.Requests.Users;
using ChatApp.Domain.DTOs.Http.Responses;
using ChatApp.Domain.DTOs.Http.Responses.Users;
using ChatApp.Domain.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ChatApp.WebAPI.Controllers
{
    [Route("auth")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly IJwtService _jwtService;
        private readonly IUserContext _userContext;
        private readonly IUserService _userService;
        public AuthenticationController(UserManager<User> userManager, IJwtService jwtService, IUserContext userContext, IUserService userService)
        {
            _userManager = userManager;
            _jwtService = jwtService;
            _userContext = userContext;
            _userService = userService;
        }

        [HttpPost("change_password")]
        [Authorize]
        public async Task<IActionResult> ChangePasswordAsync([FromBody] ChangePasswordDto changePassword)
        {
            var userIdClaim = _userContext.GetUserId();
            if (string.IsNullOrEmpty(userIdClaim))
                return BadRequest(new ChangePasswordResponseDto { Success = false, Error = "Unable to retrieve user id from token." });

            if (await _userService.ChangePasswordAsync(userIdClaim, changePassword.NewPassword, changePassword.CurrentPassword))
                return Ok(new ChangePasswordResponseDto { Success = true });

            return BadRequest(new ChangePasswordResponseDto { Success = false, Error = "Wrong current password" });
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> LoginAsync([FromBody] LoginModelDto login)
        {
            var user = await _userManager.FindByNameAsync(login.UserName);
            if (user != null && await _userManager.CheckPasswordAsync(user, login.Password))
            {
                return Ok(new LoginResponseDto { Success = true, Token = _jwtService.GetToken(user.Id, login.UserName) });
            }
            return BadRequest(new LoginResponseDto {Success = false, Error = "UserName or password is wrong"});
        }
        
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> RegisterAsync([FromBody] RegisterModelDto model)
        {
            var newUser = new User { UserName = model.Username, Email = model.Email };
            var result = await _userManager.CreateAsync(newUser, model.Password);

            if (result.Succeeded)
                return Ok(new RegisterResponseDto{ Successful = true} );
            
            var errors = result.Errors.Select(x => x.Description);
            return BadRequest(new RegisterResponseDto { Successful = false, Errors = errors });
        }
    }
}
