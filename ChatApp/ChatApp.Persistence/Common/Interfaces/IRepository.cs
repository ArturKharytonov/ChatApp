using System.Linq.Expressions;

namespace ChatApp.Persistence.Common.Interfaces
{
    public interface IRepository<TEntity, in TId>
    {
        Task<TEntity?> GetByIdAsync(TId id, params Expression<Func<TEntity, object>>[] includes);
        Task<TEntity?> GetByIdAsync(TId id);
        Task<IEnumerable<TEntity>> GetAllAsync();
        Task CreateAsync(TEntity entity);
        void Update(TEntity entity);
        Task DeleteAsync(TId id);
        Task<IQueryable<TEntity>> GetAllAsQueryableAsync();
    }
}
