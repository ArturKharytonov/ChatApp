using System.Security.Claims;

namespace ChatApp.Application.Services.UserContext
{
    public class UserContext : Interfaces.IUserContext
    {
        private readonly ClaimsPrincipal _user;
        public UserContext(ClaimsPrincipal user)
        {
            _user = user;
        }

        public string? GetUserId()
        {
            var userIdClaim = _user.FindFirst(ClaimTypes.NameIdentifier);
            return userIdClaim?.Value;
        }
    }
}
