using ChatApp.Domain.Common;
using ChatApp.Persistence.Common;
using ChatApp.Persistence.Common.Interfaces;
using ChatApp.Persistence.Context;
using ChatApp.Persistence.UnitOfWork.Interfaces;

namespace ChatApp.Persistence.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ChatDbContext _context;
        public UnitOfWork(ChatDbContext context) 
            => _context = context;

        public IRepository<TEntity, TId>? GetRepository<TEntity, TId>()
            where TEntity : class, IDbEntity<TId>
        {
            return new Repository<TEntity, TId>(_context);
        }
        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
