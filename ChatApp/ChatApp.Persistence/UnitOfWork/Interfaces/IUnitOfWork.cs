using ChatApp.Persistence.Common.Interfaces;

namespace ChatApp.Persistence.UnitOfWork.Interfaces
{
    internal interface IUnitOfWork
    {
        IRepository<T>? GetRepository<T>() 
            where T : class;
        Task SaveAsync();
    }
}
