namespace SonoCap.MES.Repositories.Base
{
    public interface IRepositoryBase<T> where T : class
    {
        IQueryable<T> GetQueryable();
        Task<IEnumerable<T>> GetAllAsync();
        Task<T?> GetByIdAsync(int id);
        Task<bool> InsertAsync(T entity);
        Task<bool> BulkInsertAsync(IEnumerable<T> entities);
        Task<bool> UpdateAsync(T entity);
        Task<bool> DeleteAsync(T entity);
        Task<bool> DeleteByIdAsync(int id);
        IQueryable<T> GetBySn(string sn);
        Task<int> UpsertAsync(T entity);
        T? GetById(int id);
    }
}
