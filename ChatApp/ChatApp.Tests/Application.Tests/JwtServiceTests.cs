using ChatApp.Application.Services.JwtHandler;
using Moq;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Extensions.Configuration;
using ChatApp.Tests.Fixtures.Services;

namespace ChatApp.Tests.Application.Tests
{
    public class JwtServiceTests : IClassFixture<JwtServiceFixture>
    {
        private readonly JwtServiceFixture _fixture;

        public JwtServiceTests()
        {
            _fixture = new JwtServiceFixture();
        }

        [Theory]
        [InlineData(123, "testuser", "your-secret-key-test", "your-issuer", "your-audience")]
        public void GetToken_ShouldGenerateValidJwtToken(int userId, string username, string key, string issuer, string audience)
        {
            // Arrange
            _fixture.Setup(key, issuer, audience);

            // Act
            var token = _fixture.JwtService.GetToken(userId, username);

            // Assert
            Assert.NotNull(token);

            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadToken(token) as JwtSecurityToken;

            Assert.NotNull(jwtToken);
            Assert.Equal(issuer, jwtToken.Issuer);
            Assert.Equal(audience, jwtToken.Audiences.FirstOrDefault());

            var userIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            var usernameClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name);

            Assert.NotNull(userIdClaim);
            Assert.Equal(userId.ToString(), userIdClaim.Value);

            Assert.NotNull(usernameClaim);
            Assert.Equal(username, usernameClaim.Value);
        }
    }
}
