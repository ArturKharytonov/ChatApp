using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using ChatApp.Application.Services.UserContext;

namespace ChatApp.Tests.Application.Tests
{
    public class UserContextTests
    {
        [Fact]
        public void GetUserId_ShouldReturnUserId_WhenUserHasNameIdentifierClaim()
        {
            // Arrange
            var userIdValue = "123";
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new(ClaimTypes.NameIdentifier, userIdValue),
            }));
            var userContext = new UserContext(user);

            // Act
            var result = userContext.GetUserId();

            // Assert
            Assert.Equal(userIdValue, result);
        }

        [Fact]
        public void GetUserId_ShouldReturnNull_WhenUserDoesNotHaveNameIdentifierClaim()
        {
            // Arrange
            var user = new ClaimsPrincipal(new ClaimsIdentity());
            var userContext = new UserContext(user);

            // Act
            var result = userContext.GetUserId();

            // Assert
            Assert.Null(result);
        }
    }
}
