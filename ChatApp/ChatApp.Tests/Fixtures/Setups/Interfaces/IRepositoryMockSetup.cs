using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChatApp.Persistence.Common.Interfaces;
using Moq;

namespace ChatApp.Tests.Fixtures.Setups.Interfaces
{
    public interface IRepositoryMockSetup<TEntity, TKey>
        where TEntity : class
    {
        void SetupRepository(Mock<IRepository<TEntity, TKey>> repository, TEntity entity);
    }
}
