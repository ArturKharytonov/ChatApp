using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChatApp.Domain.Rooms;
using ChatApp.Persistence.Context;
using ChatApp.Persistence.Repositories.Rooms.Interfaces;

namespace ChatApp.Persistence.Repositories.Rooms
{
    public class RoomRepository : IRoomRepository
    {
        private ChatDbContext _context;
        public RoomRepository(ChatDbContext context)
        {
            _context = context;
        }
        public Task<Room?> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<List<Room>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task CreateAsync(Room entity)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(Room entity)
        {
            throw new NotImplementedException();
        }

        public void Delete(int id)
        {
            throw new NotImplementedException();
        }
    }
}
