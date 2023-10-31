using ChatApp.UI.ViewModels;
using ChatApp.UI.ViewModels.Responses;
using ChatApp.WebAPI.Services.JwtHandler.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using LoginResponse = ChatApp.UI.ViewModels.Responses.LoginResponse;

namespace ChatApp.WebAPI.Controllers
{
    [Route("auth")]
    [ApiController]
    [AllowAnonymous]
    public class AuthenticationController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IJwtService _jwtService;
        public AuthenticationController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, IJwtService jwtService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtService = jwtService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync([FromBody] LoginModel login)
        {
            var result = await _signInManager.PasswordSignInAsync(login.Username, login.Password,
                false, false);
            if(!result.Succeeded)
                return BadRequest(new LoginResponse {Success = false, Error = "Username or password is wrong"});
            return Ok(new LoginResponse{Success = true, Token = _jwtService.GetToken(login.Username)});
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterAsync([FromBody] RegisterModel model)
        {
            var newUser = new IdentityUser { UserName = model.Username, Email = model.Email };
            var result = await _userManager.CreateAsync(newUser, model.Password);

            if (result.Succeeded)
                return Ok(new RegisterResponse{ Successful = true});
            

            var errors = result.Errors.Select(x => x.Description);
            return BadRequest(new RegisterResponse { Successful = false, Errors = errors });
        }
    }
}
