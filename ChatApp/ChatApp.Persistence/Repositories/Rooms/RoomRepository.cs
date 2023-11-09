using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChatApp.Domain.Rooms;
using ChatApp.Persistence.Context;
using ChatApp.Persistence.Repositories.Rooms.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Persistence.Repositories.Rooms
{
    public class RoomRepository : IRoomRepository
    {
        private readonly ChatDbContext _context;
        public RoomRepository(ChatDbContext context)
        {
            _context = context;
        }
        public async Task<Room?> GetByIdAsync(int id)
        {
            return await _context.Rooms
                .Include(c => c.Messages)
                .Include(c => c.UsersAndRooms)
                .FirstOrDefaultAsync(c => c.Id.Equals(id));
        }

        public async Task<List<Room>> GetAllAsync()
        {
            return await _context.Rooms.Include(c => c.Messages).ToListAsync();
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
