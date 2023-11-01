using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChatApp.Domain.UsersAndRooms;
using ChatApp.Persistence.Context;
using ChatApp.Persistence.Repositories.UsersAndRooms.Interfaces;

namespace ChatApp.Persistence.Repositories.UsersAndRooms
{
    public class UsersAndRoomsRepository : IUsersAndRoomsRepository
    {
        private ChatDbContext _context;
        public UsersAndRoomsRepository(ChatDbContext context)
        {
            _context = context;
        }
        public Task<UsersAndRoom?> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<List<UsersAndRoom>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task CreateAsync(UsersAndRoom entity)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(UsersAndRoom entity)
        {
            throw new NotImplementedException();
        }

        public void Delete(int id)
        {
            throw new NotImplementedException();
        }
    }
}
