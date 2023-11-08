using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using ChatApp.Domain.Users;
using IUserContext = ChatApp.WebAPI.Services.UserContext.Interfaces.IUserContext;

namespace ChatApp.WebAPI.Services.UserContext
{
    public class UserContext : IUserContext
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
