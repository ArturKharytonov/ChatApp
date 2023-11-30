using ChatApp.Application.Services.UserContext.Interfaces;
using ChatApp.Application.Services.UserService.Interfaces;
using ChatApp.WebAPI.Controllers;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.Tests.Fixtures.Controllers
{
    public class UserControllerFixture : IDisposable
    {
        public readonly Mock<IUserService> UserServiceMock;
        public readonly Mock<IUserContext> UserContextMock;
        public readonly UserController UserController;

        public UserControllerFixture()
        {
            UserContextMock = new Mock<IUserContext>();
            UserServiceMock = new Mock<IUserService>();
            UserController = new UserController(UserServiceMock.Object, UserContextMock.Object);
        }

        public void Dispose() { }
    }
}
