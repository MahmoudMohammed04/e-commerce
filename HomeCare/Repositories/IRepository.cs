using Microsoft.EntityFrameworkCore.Storage;
using System.Transactions;

namespace HomeCare.Repositories
{
    public interface IRepository<T, K> where T : class
    {
        IDbContextTransaction GetTransaction();
        Task<List<T>> GetAllAsync();
        Task<T?> GetAsync(K id);
        Task<bool> IsExists(K id);
        Task AddAsync(T entity);
        Task AddRangeAsync(IEnumerable<T> entity);
        void Update(T entity);
        void UpdateRange(IEnumerable<T> entity);
        void DeleteRange(IEnumerable<T> entity);

        void Delete(T entity);

        Task SaveAsync();
    }
}