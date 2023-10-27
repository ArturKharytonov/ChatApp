using ChatApp.Persistence.Context;
using ChatApp.Persistence.Repositories.User.Interfaces;


namespace ChatApp.Persistence.Repositories.User
{
    public class UserRepository : IUserRepository
    {
        private readonly ChatDbContext _context;

        public UserRepository(ChatDbContext context)
        {
            _context = context;
        }
        public Task<Domain.Users.User?> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<List<Domain.Users.User>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task CreateAsync(Domain.Users.User entity)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(Domain.Users.User entity)
        {
            throw new NotImplementedException();
        }

        public void Delete(int id)
        {
            throw new NotImplementedException();
        }
    }
}
