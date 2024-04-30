using Microsoft.EntityFrameworkCore;

namespace MES.UI.Repositories.Base
{
    public abstract class RepositoryBase<T> : IRepositoryBase<T> where T : class
    {
        protected readonly DbContext _context;
        protected readonly DbSet<T> _dbSet;

        public RepositoryBase(DbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public async Task<bool> DeleteAsync(T entity)
        {
            _dbSet.Remove(entity);
            int count = await _context.SaveChangesAsync();

            return count > 0;
        }

        public async Task<bool> DeleteByIdAsync(int id)
        {
            T entity = await _dbSet.FindAsync(id);

            return await DeleteAsync(entity);
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task<T> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        //AsQueryable 메서드는 메모리 내에서 데이터에 대한 쿼리 가능한 개체를 만들기 때문에 비동기 작업이 필요하지 않습니다.
        public IQueryable<T> GetQueryable()
        {
            return _dbSet.AsQueryable();
        }

        public async Task<bool> InsertAsync(T entity)
        {
            _dbSet.Add(entity);
            int count = await _context.SaveChangesAsync();

            return count > 0;
        }

        public async Task<bool> UpdateAsync(T entity)
        {
            _context.Entry(entity).State = EntityState.Modified;
            int count = await _context.SaveChangesAsync();

            return count > 0;
        }
    }
}