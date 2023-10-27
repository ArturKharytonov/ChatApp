using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChatApp.Persistence.Context;
using ChatApp.Persistence.Repositories.Friend.Interfaces;

namespace ChatApp.Persistence.Repositories.Friend
{
    public class FriendRepository : IFriendRepository
    {
        private readonly ChatDbContext _context;

        public FriendRepository(ChatDbContext context)
        {
            _context = context;
        }
        public Task<Domain.Friends.Friend?> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<List<Domain.Friends.Friend>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task CreateAsync(Domain.Friends.Friend entity)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(Domain.Friends.Friend entity)
        {
            throw new NotImplementedException();
        }

        public void Delete(int id)
        {
            throw new NotImplementedException();
        }
    }
}
