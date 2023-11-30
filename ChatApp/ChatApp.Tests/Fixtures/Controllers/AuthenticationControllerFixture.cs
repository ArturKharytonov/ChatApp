using ChatApp.Application.Services.JwtHandler.Interfaces;
using ChatApp.Application.Services.UserContext.Interfaces;
using ChatApp.Application.Services.UserService.Interfaces;
using ChatApp.Domain.Users;
using ChatApp.WebAPI.Controllers;
using Microsoft.AspNetCore.Identity;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.Tests.Fixtures.Controllers
{
    public class AuthenticationControllerFixture : IDisposable
    {
        public readonly Mock<UserManager<User>> UserManagerMock;
        public readonly Mock<IJwtService> JwtServiceMock;
        public readonly Mock<IUserContext> UserContextMock;
        public readonly Mock<IUserService> UserServiceMock;
        public readonly AuthenticationController AuthenticationController;

        public AuthenticationControllerFixture()
        {
            UserManagerMock = new Mock<UserManager<User>>(Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);
            JwtServiceMock = new Mock<IJwtService>();
            UserContextMock = new Mock<IUserContext>();
            UserServiceMock = new Mock<IUserService>();
            AuthenticationController = new AuthenticationController(
                UserManagerMock.Object,
                JwtServiceMock.Object,
                UserContextMock.Object,
                UserServiceMock.Object
            );
        }

        public void Dispose() { }
    }
}
