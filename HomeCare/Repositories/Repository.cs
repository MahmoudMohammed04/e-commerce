using HomeCare.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Transactions;

namespace HomeCare.Repositories
{
    public class Repository<T, K> : IRepository<T, K> where T : class
    {
        protected readonly AppDbContext _context;
        public DbSet<T> Table => _context.Set<T>();

        public Repository(AppDbContext context)
        {
            _context = context;
        }

        public IDbContextTransaction GetTransaction()
        {
            return _context.Database.BeginTransaction();
        }
        // GET ALL
        public async Task<List<T>> GetAllAsync()
        {
            return await Table.ToListAsync();
        }

        // GET BY ID
        public async Task<T?> GetAsync(K id)
        {
            return await Table.FindAsync(id);
        }

        public async Task<bool> IsExists(K id)
        {
            return await Table.AnyAsync(x => x.Equals(id));
        }
        // ADD SINGLE
        public async Task AddAsync(T entity)
        {
            await Table.AddAsync(entity);
        }

        // ADD RANGE
        public async Task AddRangeAsync(IEnumerable<T> entity)
        {
            await Table.AddRangeAsync(entity);
        }

        // UPDATE SINGLE
        public void Update(T entity)
        {
            Table.Update(entity);
        }

        // UPDATE RANGE
        public void UpdateRange(IEnumerable<T> entity)
        {
            Table.UpdateRange(entity);
        }

        // DELETE RANGE
        public void DeleteRange(IEnumerable<T> entity)
        {
            Table.RemoveRange(entity);
        }

        public void Delete(T entity)
        {
            Table.Remove(entity);
        }

        // SAVE
        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}