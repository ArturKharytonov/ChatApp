using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChatApp.Persistence.Context;
using ChatApp.Persistence.Repositories.Message.Interfaces;

namespace ChatApp.Persistence.Repositories.Message
{
    public class MessageRepository : IMessageRepository
    {
        private ChatDbContext _context;
        public MessageRepository(ChatDbContext context)
        {
            _context = context;
        }

        public Task<Domain.Messages.Message?> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<List<Domain.Messages.Message>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task CreateAsync(Domain.Messages.Message entity)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(Domain.Messages.Message entity)
        {
            throw new NotImplementedException();
        }

        public void Delete(int id)
        {
            throw new NotImplementedException();
        }
    }
}
