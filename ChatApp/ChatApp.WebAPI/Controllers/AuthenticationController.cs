using ChatApp.Domain.DTOs.Http;
using ChatApp.Domain.DTOs.Http.Responses;
using ChatApp.Domain.Users;
using ChatApp.WebAPI.Services.JwtHandler.Interfaces;
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
        public AuthenticationController(UserManager<User> userManager, IJwtService jwtService)
        {
            _userManager = userManager;
            _jwtService = jwtService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync([FromBody] LoginModelDto login)
        {
            var user = await _userManager.FindByNameAsync(login.UserName);
            if (user != null && await _userManager.CheckPasswordAsync(user, login.Password))
            {
                return Ok(new LoginResponseDto { Success = true, Token = _jwtService.GetToken(login.UserName) });
            }
            return BadRequest(new LoginResponseDto {Success = false, Error = "UserName or password is wrong"});
        }
        
        [HttpPost("register")]
        public async Task<IActionResult> RegisterAsync([FromBody] RegisterModelDto model)
        {
            var newUser = new User { UserName = model.Username, Email = model.Email };
            var result = await _userManager.CreateAsync(newUser, model.Password);

            if (result.Succeeded)
                return Ok(new RegisterResponseDto{ Successful = true} );
            
            var errors = result.Errors.Select(x => x.Description);
            return BadRequest(new RegisterResponseDto { Successful = false, Errors = errors });
        }


        [HttpPost("test")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public IActionResult Test([FromBody] LoginModelDto model)
        {
            return Ok(new LoginResponseDto{Success = true});
        }
    }
}
