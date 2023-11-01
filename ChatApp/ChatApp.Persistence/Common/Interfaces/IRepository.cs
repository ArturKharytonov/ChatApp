namespace ChatApp.Persistence.Common.Interfaces
{
    public interface IRepository<T> 
        where T : class
    {
        Task<T?> GetByIdAsync(int id);
        Task<List<T>> GetAllAsync();
        Task CreateAsync(T entity);
        Task UpdateAsync(T entity);
        void Delete(int id);
    }
}
