using System.Security.Claims;
using IUserContext = ChatApp.WebAPI.Services.UserContext.Interfaces.IUserContext;

namespace ChatApp.WebAPI.Services.UserContext
{
    public class UserContext : IUserContext
    {
        private readonly ClaimsPrincipal _user;
        public UserContext(ClaimsPrincipal user)
            => _user = user;

        public string? GetUserId()
        {
            var userIdClaim = _user.FindFirst(ClaimTypes.NameIdentifier);
            return userIdClaim?.Value;
        }
    }
}
