using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChatApp.Domain.Common;
using ChatApp.Persistence.Common.Interfaces;
using ChatApp.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Persistence.Common
{
    public class Repository<TEntity, TId> : IRepository<TEntity, TId>
        where TEntity : class, IDbEntity<TId>
    {
        private readonly ChatDbContext _context;
        public Repository(ChatDbContext chatDbContext)
        {
            _context = chatDbContext;
        }

        public async Task<TEntity?> GetByIdAsync(TId id)
        {
            return await _context.Set<TEntity>().FindAsync(id);
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            return await _context.Set<TEntity>().ToListAsync();
        }
        public async Task CreateAsync(TEntity entity)
        {
            await _context.Set<TEntity>().AddAsync(entity);
        }
        public async Task<IQueryable<TEntity>> GetAllAsQueryableAsync()
        {
            var query = _context.Set<TEntity>().AsQueryable();

            return await Task.FromResult(query);
        }
        public void Update(TEntity entity)
        {
            _context.Set<TEntity>().Update(entity);
        }

        public async Task DeleteAsync(TId id)
        {
            await _context.Set<TEntity>()
                .Where(entity => entity.Id.Equals(id))
                .ExecuteDeleteAsync();
        }
    }
}
