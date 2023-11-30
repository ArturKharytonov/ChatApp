using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using ChatApp.Persistence.Common.Interfaces;
using ChatApp.Tests.Fixtures.Setups.Interfaces;
using Moq;

namespace ChatApp.Tests.Fixtures.Setups
{
    public class RepositoryMockSetup<TEntity, TKey> : IRepositoryMockSetup<TEntity, TKey>
        where TEntity : class
    {
        public void SetupRepository(Mock<IRepository<TEntity, TKey>> repository, TEntity entity)
        {
            repository.Setup(repo => repo.GetByIdAsync(It.IsAny<TKey>(), It.IsAny<Expression<Func<TEntity, object>>[]>()))
                .ReturnsAsync(entity);
        }
    }
}
