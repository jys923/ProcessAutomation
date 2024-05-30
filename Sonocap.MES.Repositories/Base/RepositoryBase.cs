using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
using Serilog;
using SonoCap.MES.Models.Base;

namespace SonoCap.MES.Repositories.Base
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

        public IQueryable<T> GetBySn(string sn)
        {
            if (typeof(ISn).IsAssignableFrom(typeof(T)))
            {
                return _dbSet.OfType<ISn>().Where(e => e.Sn.Contains(sn)).Cast<T>();
            }

            throw new InvalidOperationException("T does not implement IHasSn interface.");
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

        public async Task<bool> BulkInsertAsync(IEnumerable<T> entities)
        {
            bool result = false;

            try
            {
                await _context.BulkInsertAsync(entities);
                result = true;
            }
            catch (Exception ex)
            {
                Log.Information($"BulkInsertAsync 작업 중 오류 발생: {ex.Message}");
                result = false;
            }

            return result;
        }

        public async Task<bool> UpdateAsync(T entity)
        {
            _context.Entry(entity).State = EntityState.Modified;
            int count = await _context.SaveChangesAsync();

            return count > 0;
        }
    }
}