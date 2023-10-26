using Persistence.Common.Interfaces;
using Persistence.User.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.UnitOfWork.Interfaces
{
    internal interface IUnitOfWork
    {

        IRepository<T>? GetRepository<T>() 
            where T : class;
        Task SaveAsync();
    }
}
