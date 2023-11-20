using ChatApp.Domain.Common;
using ChatApp.Persistence.Common.Interfaces;

namespace ChatApp.Persistence.UnitOfWork.Interfaces
{
    public interface IUnitOfWork
    {
        IRepository<TEntity, TId>? GetRepository<TEntity, TId>()
            where TEntity : class, IDbEntity<TId>;

        Task SaveAsync();
    }
}
