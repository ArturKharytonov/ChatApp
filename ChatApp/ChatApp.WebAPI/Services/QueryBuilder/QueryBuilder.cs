using System.Linq.Expressions;
using ChatApp.WebAPI.Services.QueryBuilder.Interfaces;
using Radzen;

namespace ChatApp.WebAPI.Services.QueryBuilder
{
    public class QueryBuilder<TEntity> : IQueryBuilder<TEntity>
    {
        public Expression<Func<TEntity, bool>> SearchQuery(string searchValue, params string[] namesOfProperties)
        {
            var parameter = Expression.Parameter(typeof(TEntity), "x");

            Expression? expression = null;

            foreach (var name in namesOfProperties)
            {
                var containsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) });

                var property = Expression.Property(parameter, name);
                var propertyAsObject = Expression.Convert(property, typeof(object));
                var nullCheck = Expression.ReferenceEqual(propertyAsObject, Expression.Constant(null));

                Expression stringified = property.Type == typeof(string)
                    ? property
                    : Expression.Call(property, property.Type.GetMethod("ToString", Type.EmptyTypes));

                var containsCall = Expression.Call(stringified, containsMethod, Expression.Constant(searchValue));
                var conditionalExpression = Expression.Condition(nullCheck, Expression.Constant(false), containsCall);

                if (expression == null)
                    expression = conditionalExpression; //containsCall
                else
                    expression = Expression.OrElse(expression, conditionalExpression); //containsCall
            }
            return Expression.Lambda<Func<TEntity, bool>>(expression!, parameter);
        }
        public IQueryable<TEntity> OrderByQuery(IQueryable<TEntity> source, string orderByValue, bool orderByType)
        {
            var command = orderByType ? "OrderBy" : "OrderByDescending";
            var type = typeof(TEntity);

            var property = type.GetProperty(orderByValue);
            var parameter = Expression.Parameter(type, "p");

            var propertyAccess = Expression.MakeMemberAccess(parameter, property);
            var orderByExpression = Expression.Lambda(propertyAccess, parameter);
            var resultExpression = Expression.Call(typeof(Queryable), command, new Type[] { type, property.PropertyType },
                source.Expression, Expression.Quote(orderByExpression));
            return source.Provider.CreateQuery<TEntity>(resultExpression);
        }
    }
}
