using SonoCap.MES.Models;
using SonoCap.MES.Repositories.Base;

namespace SonoCap.MES.Repositories.Interfaces
{
    public interface IProbeRepository : IRepositoryBase<Probe>
    {
        Task<PTRView?> GetPTRViewAsync(string probeSn);
        //Task<int> SetPTRViewsAsync();
    }
}