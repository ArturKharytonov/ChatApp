using ChatApp.Application.Services.JwtHandler;
using Moq;
using Microsoft.Extensions.Configuration;

namespace ChatApp.Tests.Fixtures.Services
{
    public class JwtServiceFixture
    {
        private readonly Mock<IConfiguration> _configurationMock;
        public readonly JwtService JwtService;

        public JwtServiceFixture()
        {
            _configurationMock = new Mock<IConfiguration>();
            JwtService = new JwtService(_configurationMock.Object);
        }

        public void Dispose() { }

        public void Setup(string key, string issuer, string audience)
        {
            _configurationMock.SetupGet(x => x["Jwt:Key"]).Returns(key);
            _configurationMock.SetupGet(x => x["Jwt:Issuer"]).Returns(issuer);
            _configurationMock.SetupGet(x => x["Jwt:Audience"]).Returns(audience);
        }
    }
}
