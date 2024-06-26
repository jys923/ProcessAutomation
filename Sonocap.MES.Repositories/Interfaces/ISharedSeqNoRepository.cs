using SonoCap.MES.Models;
using SonoCap.MES.Models.Enums;
using SonoCap.MES.Repositories.Base;

namespace SonoCap.MES.Repositories.Interfaces
{
    public interface ISharedSeqNoRepository : IRepositoryBase<SharedSeqNo>
    {
        Task<SharedSeqNo?> GetSeqNoAsync(DateTime dateTime = default);
        Task InitializeAsync();
        Task<bool> SetSeqNoAsync(DateTime dateTime = default);
        Task<bool> SetSeqNoAsync(SnType type, DateTime dateTime = default);
        Task<SharedSeqNo?> UpsertSeqNoAsync(SnType type, DateTime dateTime = default);
    }
}