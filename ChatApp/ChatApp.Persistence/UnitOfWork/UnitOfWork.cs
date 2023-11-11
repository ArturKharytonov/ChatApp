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
        //private readonly Dictionary<Type, object> _repositories = new();
        public UnitOfWork(ChatDbContext context) 
            => _context = context;

        public IRepository<TEntity, TId>? GetRepository<TEntity, TId>()
            where TEntity : class, IDbEntity<TId>
        {
            //var entityType = typeof(TEntity);
            //if (_repositories.TryGetValue(entityType, out var repository))
            //{
            //    return (IRepository<TEntity, TId>)repository;
            //}

            //var repositoryType = typeof(Repository<TEntity, TId>);
            //var newRepository = Activator.CreateInstance(repositoryType, _context);
            //_repositories.Add(entityType, newRepository);

            //return (IRepository<TEntity, TId>)newRepository;

            return new Repository<TEntity, TId>(_context);
        }
        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
