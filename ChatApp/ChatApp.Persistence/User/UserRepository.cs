using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Persistence.User.Interfaces;

namespace Persistence.User
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;

        public UserRepository(ApplicationDbContext context)
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
