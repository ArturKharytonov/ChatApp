using ChatApp.Persistence.Common.Interfaces;

namespace ChatApp.Persistence.Repositories.User.Interfaces
{
    public interface IUserRepository : IRepository<Domain.Users.User>
    {
        // add some additional functional for user
    }
}
