using System.Linq.Expressions;

namespace ChatApp.WebAPI.Services.QueryBuilder.Interfaces
{
    public interface IQueryBuilder<TEntity>
    {
        Expression<Func<TEntity, bool>> SearchQuery(string searchValue, params string[] names);
        IQueryable<TEntity> OrderByQuery(IQueryable<TEntity> source, string orderByValue, bool orderByType);
    }
}
