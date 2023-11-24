using System.Diagnostics.CodeAnalysis;
using ChatApp.Domain.Common;
using ChatApp.Persistence.Common.Interfaces;
using ChatApp.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

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
        public async Task<TEntity?> GetByIdAsync(TId id, params Expression<Func<TEntity, object>>[] includes)
        {
            var query = _context.Set<TEntity>().AsQueryable();

            query = includes.Aggregate(query, (current, include) =>
                current.Include(include));

            return await query.FirstOrDefaultAsync(entity => EF.Property<TId>(entity, "Id")!.Equals(id));
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
