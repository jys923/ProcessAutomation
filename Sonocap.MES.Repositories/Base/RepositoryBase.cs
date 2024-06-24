using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
using Serilog;
using SonoCap.MES.Models.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SonoCap.MES.Repositories.Base
{
    public abstract class RepositoryBase<T> : IRepositoryBase<T> where T : ModelBase
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
            if (entity == null)
                return false;

            return await DeleteAsync(entity);
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public T? GetById(int id)
        {
            return _dbSet.Where(x => x.Id == id).OrderByDescending(x => x.Id).FirstOrDefault();
        }

        public async Task<T?> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public IQueryable<T> GetBySn(string sn)
        {
            if (typeof(ISn).IsAssignableFrom(typeof(T)))
            {
                return _dbSet.OfType<ISn>().Where(e => e.Sn.Equals(sn)).Cast<T>();
            }

            throw new InvalidOperationException("T does not implement ISn interface.");
        }

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

        public async Task<int> UpsertAsync(T entity)
        {
            var existingEntity = await _dbSet.FindAsync(entity.Id);

            if (existingEntity != null )
            {
                _context.Entry(existingEntity).CurrentValues.SetValues(entity);
            }
            else
            {
                _dbSet.Add(entity);
            }

            int count = await _context.SaveChangesAsync();
            return entity.Id; // Insert 시 새로 생성된 Id 반환
            //return count > 0;
        }

        public async Task<bool> BulkInsertAsync(IEnumerable<T> entities)
        {
            try
            {
                await _context.BulkInsertAsync(entities);
                return true;
            }
            catch (Exception ex)
            {
                Log.Error($"Error during BulkInsertAsync operation: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> UpdateAsync(T entity)
        {
            _context.Entry(entity).State = EntityState.Modified;
            int count = await _context.SaveChangesAsync();
            return count > 0;
        }
    }
}
